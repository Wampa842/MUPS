using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PmxSharp
{
    public class PmxImporter : IDisposable
    {
        public Encoding TextEncoding { get; private set; } = Encoding.Unicode;
        public string FilePath { get; set; }
        private BinaryReader _reader;
        private PmxModel _model;

        public PmxModel Import()
        {
            _model = new PmxModel(Path.GetFileName(FilePath));
            _model.FilePath = FilePath;

            #region Header
            // Signature (4 bytes)
            if (!PmxVersionCheck.FileIsPmx(_reader.ReadBytes(4)))
                throw new PmxException("File is not PMX!");

            // Version (float)
            switch (_reader.ReadSingle())
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
            byte globalsCount = _reader.ReadByte();
            if (_model.Version == PmxVersion.Pmx20)
                globalsCount = 8;
            for(byte i = 0; i < 8; ++i)
            {
                switch(i)
                {
                    case 0:
                        TextEncoding = _reader.ReadByte() == 0 ? Encoding.Unicode : Encoding.UTF8;
                        break;
                    case 1:
                        _model.AdditionalUVCount = _reader.ReadByte();
                        break;
                    case 2:
                        PmxIndex.Vertex = _reader.ReadByte();
                        break;
                    case 3:
                        PmxIndex.Texture = _reader.ReadByte();
                        break;
                    case 4:
                        PmxIndex.Material = _reader.ReadByte();
                        break;
                    case 5:
                        PmxIndex.Bone = _reader.ReadByte();
                        break;
                    case 6:
                        PmxIndex.Morph = _reader.ReadByte();
                        break;
                    case 7:
                        PmxIndex.Rigidbody = _reader.ReadByte();
                        break;
                    default:
                        break;
                }
            }

            // Name
            _model.NameJapanese = _reader.ReadPmxString(TextEncoding);
            _model.NameEnglish = _reader.ReadPmxString(TextEncoding);
            _model.DescriptionJapanese = _reader.ReadPmxString(TextEncoding);
            _model.DescriptionEnglish = _reader.ReadPmxString(TextEncoding);
            #endregion

            #region Vertex data
            _model.Vertices = new PmxVertex[_reader.ReadInt32()];
            for(int i = 0; i < _model.Vertices.Length; ++i)
            {
                PmxVertex vert = new PmxVertex();

                // Standard properties
                vert.Position = _reader.ReadVector3();
                vert.Normal = _reader.ReadVector3();
                vert.UV = _reader.ReadVector2();

                // Additional UVs
                vert.AdditionalUVs = new UnityEngine.Vector4[_model.AdditionalUVCount];
                for(int j = 0; j < _model.AdditionalUVCount; ++j)
                {
                    vert.AdditionalUVs[j] = _reader.ReadVector4();
                }

                // Deform
                switch (_reader.ReadByte())
                {
                    case 0:
                        vert.Deform = new Bdef1Deform(_reader.ReadIndex(PmxIndex.Bone));
                        break;
                    case 1:
                        vert.Deform = new Bdef2Deform
                        {
                            Bone0 = _reader.ReadIndex(PmxIndex.Bone),
                            Bone1 = _reader.ReadIndex(PmxIndex.Bone),
                            Weight0 = _reader.ReadSingle()
                        };
                        break;
                    case 2:
                        vert.Deform = new Bdef4Deform
                        {
                            Bone0 = _reader.ReadIndex(PmxIndex.Bone),
                            Bone1 = _reader.ReadIndex(PmxIndex.Bone),
                            Bone2 = _reader.ReadIndex(PmxIndex.Bone),
                            Bone3 = _reader.ReadIndex(PmxIndex.Bone),
                            Weight0 = _reader.ReadSingle(),
                            Weight1 = _reader.ReadSingle(),
                            Weight2 = _reader.ReadSingle(),
                            Weight3 = _reader.ReadSingle()
                        };
                        break;
                    case 3:
                        vert.Deform = new SdefDeform
                        {
                            Bone0 = _reader.ReadIndex(PmxIndex.Bone),
                            Bone1 = _reader.ReadIndex(PmxIndex.Bone),
                            Weight0 = _reader.ReadSingle(),
                            C = _reader.ReadVector3(),
                            R0 = _reader.ReadVector3(),
                            R1 = _reader.ReadVector3()
                        };
                        break;
                    case 4:
                        vert.Deform = new QdefDeform
                        {
                            Bone0 = _reader.ReadIndex(PmxIndex.Bone),
                            Bone1 = _reader.ReadIndex(PmxIndex.Bone),
                            Bone2 = _reader.ReadIndex(PmxIndex.Bone),
                            Bone3 = _reader.ReadIndex(PmxIndex.Bone),
                            Weight0 = _reader.ReadSingle(),
                            Weight1 = _reader.ReadSingle(),
                            Weight2 = _reader.ReadSingle(),
                            Weight3 = _reader.ReadSingle()
                        };
                        break;
                    default:
                        break;
                }

                // Edge
                vert.EdgeSize = _reader.ReadSingle();

                _model.Vertices[i] = vert;
            }
            #endregion

            #region Surface data
            _model.Surfaces = new PmxSurface[_reader.ReadInt32()];
            for(int i = 0; i < _model.Surfaces.Length; ++i)
            {
                _model.Surfaces[i] = new PmxSurface(_reader.ReadIndex(PmxIndex.Vertex, false), _reader.ReadIndex(PmxIndex.Vertex, false), _reader.ReadIndex(PmxIndex.Vertex, false));
            }
            #endregion

            #region Texture data
            _model.TexturePaths = new string[_reader.ReadInt32()];
            for(int i = 0; i < _model.TexturePaths.Length; ++i)
            {
                _model.TexturePaths[i] = _reader.ReadPmxString(TextEncoding);
            }
            #endregion

            #region Material data
            _model.Materials = new PmxMaterial[_reader.ReadInt32()];
            for(int i = 0; i < _model.Materials.Length; ++i)
            {
                PmxMaterial mat = new PmxMaterial();
                mat.NameJapanese = _reader.ReadPmxString(TextEncoding);
                mat.NameEnglish = _reader.ReadPmxString(TextEncoding);

                mat.Diffuse = _reader.ReadColor4();
                mat.Specular = _reader.ReadColor3();
                mat.SpecularExponent = _reader.ReadSingle();
                mat.Ambient = _reader.ReadColor3();
                mat.Flags = (PmxMaterialFlags)_reader.ReadByte();
                mat.EdgeColor = _reader.ReadColor4();
                mat.EdgeSize = _reader.ReadSingle();
                mat.TextureIndex = _reader.ReadIndex(PmxIndex.Texture);
                mat.EnvironmentIndex = _reader.ReadIndex(PmxIndex.Texture);
                mat.EnvironmentType = (PmxEnvironmentTextureType)_reader.ReadByte();
                mat.ToonType = (PmxToonType)_reader.ReadByte();
                mat.ToonReference = mat.ToonType == PmxToonType.Texture ? _reader.ReadIndex(PmxIndex.Texture) : _reader.ReadByte();
                mat.Note = _reader.ReadPmxString(TextEncoding);
                mat.SurfaceCount = _reader.ReadInt32();
            }
            #endregion

            #region Skeleton data

            #endregion

            #region Morph data

            #endregion

            #region Group data

            #endregion

            #region Rigid body data

            #endregion

            #region Joint data

            #endregion

            // Version 2.0 files end here
            if (_model.Version == PmxVersion.Pmx20)
                return _model;

            #region Soft body

            #endregion

            return _model;
        }

        #region Constructors and destructors
        public PmxImporter(string path)
        {
            FilePath = path;
            _reader = new BinaryReader(File.OpenRead(path));
        }

        public PmxImporter(Stream stream)
        {
            _reader = new BinaryReader(stream);
        }

        public void Dispose()
        {
            if (_reader != null)
                _reader.Close();
        }
        #endregion
    }
}
