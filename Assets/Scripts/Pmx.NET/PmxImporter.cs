using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

namespace PmxSharp
{
    public class PmxImporter : IDisposable
    {
        public string FilePath { get; set; }
        private Encoding _textEncoding = Encoding.Unicode;
        private BinaryReader _fileReader;
        private PmxModel _model;

        private PmxModel Import(BinaryReader reader)
        {
            _model = new PmxModel("PMX model " + this.GetHashCode());
            _model.FilePath = FilePath;

            #region Header
            // Signature (4 bytes)
            if (!PmxVersionCheck.FileIsPmx(reader.ReadBytes(4)))
                throw new PmxException("File is not PMX!");

            // Version (float)
            switch (reader.ReadSingle())
            {
                case 2.0f:
                    _model.Version = PmxVersion.Pmx20;
                    break;
                case 2.1f:
                    _model.Version = PmxVersion.Pmx21;
                    break;
                default:
                    throw new PmxException("Version is not supported!");
            }

            // Globals
            byte globalsCount = reader.ReadByte();
            if (_model.Version == PmxVersion.Pmx20)
                globalsCount = 8;
            for (byte i = 0; i < 8; ++i)
            {
                switch (i)
                {
                    case 0:
                        _textEncoding = reader.ReadByte() == 0 ? Encoding.Unicode : Encoding.UTF8;
                        break;
                    case 1:
                        _model.AdditionalUVCount = reader.ReadByte();
                        break;
                    case 2:
                        PmxIndex.Vertex = reader.ReadByte();
                        break;
                    case 3:
                        PmxIndex.Texture = reader.ReadByte();
                        break;
                    case 4:
                        PmxIndex.Material = reader.ReadByte();
                        break;
                    case 5:
                        PmxIndex.Bone = reader.ReadByte();
                        break;
                    case 6:
                        PmxIndex.Morph = reader.ReadByte();
                        break;
                    case 7:
                        PmxIndex.Rigidbody = reader.ReadByte();
                        break;
                    default:
                        break;
                }
            }

            // Name
            _model.NameJapanese = reader.ReadPmxString(_textEncoding);
            _model.NameEnglish = reader.ReadPmxString(_textEncoding);
            _model.DescriptionJapanese = reader.ReadPmxString(_textEncoding);
            _model.DescriptionEnglish = reader.ReadPmxString(_textEncoding);
            #endregion

            #region Vertex data
            _model.Vertices = new PmxVertex[reader.ReadInt32()];
            for (int i = 0; i < _model.Vertices.Length; ++i)
            {
                PmxVertex vert = new PmxVertex();

                // Standard properties
                vert.Position = reader.ReadVector3();
                vert.Normal = reader.ReadVector3();
                vert.UV = reader.ReadVector2() * new Vector2(1, -1);

                // Additional UVs
                vert.AdditionalUVs = new UnityEngine.Vector4[_model.AdditionalUVCount];
                for (int j = 0; j < _model.AdditionalUVCount; ++j)
                {
                    vert.AdditionalUVs[j] = reader.ReadVector4();
                }

                // Deform
                byte deformType = reader.ReadByte();
                switch (deformType)
                {
                    case 0:
                        vert.Deform = new Bdef1Deform(reader.ReadIndex(PmxIndexType.Bone));
                        break;
                    case 1:
                        vert.Deform = new Bdef2Deform
                        {
                            Bone0 = reader.ReadIndex(PmxIndexType.Bone),
                            Bone1 = reader.ReadIndex(PmxIndexType.Bone),
                            Weight0 = reader.ReadSingle()
                        };
                        break;
                    case 2:
                        vert.Deform = new Bdef4Deform
                        {
                            Bone0 = reader.ReadIndex(PmxIndexType.Bone),
                            Bone1 = reader.ReadIndex(PmxIndexType.Bone),
                            Bone2 = reader.ReadIndex(PmxIndexType.Bone),
                            Bone3 = reader.ReadIndex(PmxIndexType.Bone),
                            Weight0 = reader.ReadSingle(),
                            Weight1 = reader.ReadSingle(),
                            Weight2 = reader.ReadSingle(),
                            Weight3 = reader.ReadSingle()
                        };
                        break;
                    case 3:
                        vert.Deform = new SdefDeform
                        {
                            Bone0 = reader.ReadIndex(PmxIndexType.Bone),
                            Bone1 = reader.ReadIndex(PmxIndexType.Bone),
                            Weight0 = reader.ReadSingle(),
                            C = reader.ReadVector3(),
                            R0 = reader.ReadVector3(),
                            R1 = reader.ReadVector3()
                        };
                        break;
                    case 4:
                        vert.Deform = new QdefDeform
                        {
                            Bone0 = reader.ReadIndex(PmxIndexType.Bone),
                            Bone1 = reader.ReadIndex(PmxIndexType.Bone),
                            Bone2 = reader.ReadIndex(PmxIndexType.Bone),
                            Bone3 = reader.ReadIndex(PmxIndexType.Bone),
                            Weight0 = reader.ReadSingle(),
                            Weight1 = reader.ReadSingle(),
                            Weight2 = reader.ReadSingle(),
                            Weight3 = reader.ReadSingle()
                        };
                        break;
                    default:
                        throw new PmxImportException(string.Format("PMX import error: vertex deform is an invalid type. Expected 0..4, got {0}.", deformType), reader.BaseStream.Position);
                }

                // Edge
                vert.EdgeSize = reader.ReadSingle();

                _model.Vertices[i] = vert;
            }
            #endregion

            #region Surface data
            _model.Surfaces = new PmxSurface[reader.ReadInt32() / 3];
            for (int i = 0; i < _model.Surfaces.Length; ++i)
            {
                _model.Surfaces[i] = new PmxSurface(reader.ReadIndex(PmxIndexType.Vertex), reader.ReadIndex(PmxIndexType.Vertex), reader.ReadIndex(PmxIndexType.Vertex));
            }
            #endregion

            #region Texture data
            _model.TexturePaths = new string[reader.ReadInt32()];
            for (int i = 0; i < _model.TexturePaths.Length; ++i)
            {
                _model.TexturePaths[i] = reader.ReadPmxString(_textEncoding);
                //Debug.Log(_model.TexturePaths[i]);
            }
            #endregion

            #region Material data
            _model.Materials = new PmxMaterial[reader.ReadInt32()];
            int assignedVertexCount = 0;
            for (int i = 0; i < _model.Materials.Length; ++i)
            {
                PmxMaterial mat = new PmxMaterial();
                mat.NameJapanese = reader.ReadPmxString(_textEncoding);
                mat.NameEnglish = reader.ReadPmxString(_textEncoding);

                mat.Diffuse = reader.ReadColor4();
                mat.Specular = reader.ReadColor3();
                mat.SpecularExponent = reader.ReadSingle();
                mat.Ambient = reader.ReadColor3();
                mat.Flags = (PmxMaterialFlags)reader.ReadByte();
                mat.EdgeColor = reader.ReadColor4();
                mat.EdgeSize = reader.ReadSingle();
                mat.TextureIndex = reader.ReadIndex(PmxIndexType.Texture);
                mat.EnvironmentIndex = reader.ReadIndex(PmxIndexType.Texture);
                mat.EnvironmentType = (PmxEnvironmentTextureType)reader.ReadByte();
                mat.ToonType = (PmxToonType)reader.ReadByte();
                mat.ToonReference = mat.ToonType == PmxToonType.Texture ? reader.ReadIndex(PmxIndex.Texture) : reader.ReadByte();
                mat.Note = reader.ReadPmxString(_textEncoding);
                mat.VertexCount = reader.ReadInt32();
                mat.FirstVertex = assignedVertexCount;

                assignedVertexCount += mat.VertexCount;
                _model.Materials[i] = mat;
                //Debug.Log(mat);
            }
            #endregion

            #region Skeleton data
            _model.Bones = new PmxBone[reader.ReadInt32()];
            for (int i = 0; i < _model.Bones.Length; ++i)
            {
                PmxBone bone = new PmxBone();

                bone.Index = i;
                bone.Children = new List<PmxBone>();
                bone.NameJapanese = reader.ReadPmxString(_textEncoding);
                bone.NameEnglish = reader.ReadPmxString(_textEncoding);
                bone.Position = reader.ReadVector3();
                bone.ParentIndex = reader.ReadIndex(PmxIndexType.Bone);
                bone.DeformOrder = reader.ReadInt32();
                bone.Flags = (PmxBoneFlags)reader.ReadUInt16();
                if (bone.HasFlag(PmxBoneFlags.TailIsIndex))
                {
                    bone.TailIndex = reader.ReadIndex(PmxIndexType.Bone);
                }
                else
                {
                    bone.TailPosition = reader.ReadVector3();
                }
                if (bone.HasFlag(PmxBoneFlags.InheritRotation | PmxBoneFlags.InheritTranslation))
                {
                    bone.InheritBone = reader.ReadIndex(PmxIndexType.Bone);
                    bone.InheritMultiplier = reader.ReadSingle();
                }
                if (bone.HasFlag(PmxBoneFlags.FixedAxis))
                {
                    bone.FixedAxis = reader.ReadVector3();
                }
                if (bone.HasFlag(PmxBoneFlags.LocalCoordinateSystem))
                {
                    bone.LocalCoordinateX = reader.ReadVector3();
                    bone.LocalCoordinateZ = reader.ReadVector3();
                }
                if (bone.HasFlag(PmxBoneFlags.ExternalParentDeform))
                {
                    bone.ExternalParent = reader.ReadIndex(PmxIndexType.Bone);
                }
                if (bone.HasFlag(PmxBoneFlags.IK))
                {
                    PmxIK ik = new PmxIK();

                    ik.Target = reader.ReadIndex(PmxIndexType.Bone);
                    ik.Loop = reader.ReadInt32();
                    ik.Limit = reader.ReadSingle();
                    ik.Links = new PmxIKLink[reader.ReadInt32()];
                    for (int j = 0; j < ik.Links.Length; ++j)
                    {
                        ik.Links[j] = new PmxIKLink();
                        ik.Links[j].Bone = reader.ReadIndex(PmxIndexType.Bone);
                        ik.Links[j].Limited = reader.ReadByte() != 0;
                        if (ik.Links[j].Limited)
                        {
                            ik.Links[j].LimitMin = reader.ReadVector3();
                            ik.Links[j].LimitMax = reader.ReadVector3();
                        }
                    }
                }

                _model.Bones[i] = bone;
            }

            foreach (PmxBone bone in _model.Bones)
            {
                if (bone.ParentIndex >= 0)
                {
                    bone.ParentBone = _model.Bones[bone.ParentIndex];
                    bone.ParentBone.Children.Add(bone);
                }
            }
            #endregion

            #region Morph data
            _model.Morphs = new PmxMorph[reader.ReadInt32()];
            for (int i = 0; i < _model.Morphs.Length; ++i)
            {
                PmxMorph morph = new PmxMorph();
                morph.NameJapanese = reader.ReadPmxString(_textEncoding);
                morph.NameEnglish = reader.ReadPmxString(_textEncoding);
                morph.Panel = (PmxMorphPanel)reader.ReadByte();
                morph.Type = (PmxMorphType)reader.ReadByte();
                morph.Offsets = new PmxMorphOffset[reader.ReadInt32()];

                switch (morph.Type)
                {
                    case PmxMorphType.Group:
                        for (int j = 0; j < morph.Offsets.Length; ++j)
                        {
                            morph.Offsets[j] = PmxGroupOffset.Read(reader);
                        }
                        break;
                    case PmxMorphType.Vertex:
                        for (int j = 0; j < morph.Offsets.Length; ++j)
                        {
                            morph.Offsets[j] = PmxVertexOffset.Read(reader);
                        }
                        break;
                    case PmxMorphType.Bone:
                        for (int j = 0; j < morph.Offsets.Length; ++j)
                        {
                            morph.Offsets[j] = PmxBoneOffset.Read(reader);
                        }
                        break;
                    case PmxMorphType.UV:
                    case PmxMorphType.UV1:
                    case PmxMorphType.UV2:
                    case PmxMorphType.UV3:
                    case PmxMorphType.UV4:
                        for (int j = 0; j < morph.Offsets.Length; ++j)
                        {
                            morph.Offsets[j] = PmxUVOffset.Read(reader);
                        }
                        break;
                    case PmxMorphType.Material:
                        for (int j = 0; j < morph.Offsets.Length; ++j)
                        {
                            morph.Offsets[j] = PmxMaterialOffset.Read(reader);
                        }
                        break;
                    case PmxMorphType.Flip:
                        for (int j = 0; j < morph.Offsets.Length; ++j)
                        {
                            morph.Offsets[j] = PmxFlipOffset.Read(reader);
                        }
                        break;
                    case PmxMorphType.Impulse:
                        for (int j = 0; j < morph.Offsets.Length; ++j)
                        {
                            morph.Offsets[j] = PmxImpulseOffset.Read(reader);
                        }
                        break;
                    default:
                        throw new PmxImportException(string.Format("Unexpected morph type. Expected 0..10, got {0}.", (int)morph.Type), reader.BaseStream.Position);
                }

                _model.Morphs[i] = morph;
            }
            #endregion

            #region Group data
            _model.DisplayFrames = new PmxFrameGroup[reader.ReadInt32()];
            for (int i = 0; i < _model.DisplayFrames.Length; ++i)
            {
                PmxFrameGroup frame = new PmxFrameGroup();
                frame.NameJapanese = reader.ReadPmxString(_textEncoding);
                frame.NameEnglish = reader.ReadPmxString(_textEncoding);
                frame.Special = reader.ReadByte() != 0;
                frame.Frames = new PmxFrameData[reader.ReadInt32()];
                for (int j = 0; j < frame.Frames.Length; ++j)
                {
                    PmxFrameData fd = new PmxFrameData();
                    fd.Type = reader.ReadByte() == 0 ? PmxFrameType.Bone : PmxFrameType.Morph;
                    fd.Index = fd.Type == PmxFrameType.Bone ? reader.ReadIndex(PmxIndexType.Bone) : reader.ReadIndex(PmxIndexType.Morph);
                    frame.Frames[j] = fd;
                }
                _model.DisplayFrames[i] = frame;
            }
            #endregion

            #region Rigid body data
            _model.Rigidbodies = new PmxRigidbody[reader.ReadInt32()];
            for(int i = 0; i < _model.Rigidbodies.Length; ++i)
            {
                PmxRigidbody rb = new PmxRigidbody();
                rb.NameJapanese = reader.ReadPmxString(_textEncoding);
                rb.NameEnglish = reader.ReadPmxString(_textEncoding);
                rb.BoneIndex = reader.ReadIndex(PmxIndexType.Bone);
                rb.CollisionGroup = reader.ReadByte();
                rb.CollisionIgnoreMask = reader.ReadInt16();
                rb.Shape = (PmxRigidbodyShape)reader.ReadByte();
                rb.Bounds = reader.ReadVector3();
                rb.Position = reader.ReadVector3();
                rb.Rotation = Quaternion.Euler(reader.ReadVector3());
                rb.Mass = reader.ReadSingle();
                rb.Drag = reader.ReadSingle();
                rb.RotationDrag = reader.ReadSingle();
                rb.Repulsion = reader.ReadSingle();
                rb.Friction = reader.ReadSingle();
                rb.PhysicsMode = (PmxRigidbodyPhysics)reader.ReadByte();
                _model.Rigidbodies[i] = rb;
            }
            #endregion

            #region Joint data
            _model.Joints = new PmxJoint[reader.ReadInt32()];
            for(int i = 0; i < _model.Joints.Length; ++i)
            {
                PmxJoint j = new PmxJoint();
                j.NameJapanese = reader.ReadPmxString(_textEncoding);
                j.NameEnglish = reader.ReadPmxString(_textEncoding);
                j.Type = (PmxJointType)reader.ReadByte();
                j.Rigidbody0 = reader.ReadIndex(PmxIndexType.Rigidbody);
                j.Rigidbody1 = reader.ReadIndex(PmxIndexType.Rigidbody);
                j.Position = reader.ReadVector3();
                j.Rotation = Quaternion.Euler(reader.ReadVector3());
                j.TranslationMin = reader.ReadVector3();
                j.TranslationMax = reader.ReadVector3();
                j.RotationMin = reader.ReadVector3();
                j.RotationMax = reader.ReadVector3();
                j.SpringTranslation = reader.ReadVector3();
                j.SpringRotation = reader.ReadVector3();
                _model.Joints[i] = j;
            }
            #endregion

            // Version 2.0 file ends here.
            if (_model.Version == PmxVersion.Pmx20)
            {
                MUPS.Log.Debug($"Finished importing the file. Stream position is {reader.BaseStream.Position}. Stream length is {reader.BaseStream.Length}.");
                return _model;
            }

            #region Soft body

            #endregion

            return _model;
        }

        public PmxModel Import()
        {
            try
            {
                _fileReader = new BinaryReader(File.OpenRead(FilePath));
                return Import(_fileReader);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (_fileReader != null)
                {
                    _fileReader.Close();
                    _fileReader = null;
                }
            }
        }

        #region Constructors and destructors
        public PmxImporter(string path)
        {
            FilePath = path;
        }

        public void Dispose()
        {
            if (_fileReader != null)
            {
                _fileReader.Close();
                _fileReader = null;
            }
        }
        #endregion
    }
}
