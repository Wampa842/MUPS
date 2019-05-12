using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using UnityEngine;
using MUPS;

namespace PmxSharp
{
    public enum ImageFileFormat { Unsupported, Png, Jpg, Bmp, Tga, Dds, Tif }

    /// <summary>
    /// Methods to load various image formats.
    /// </summary>
    public static class ImageLoader
    {
        private readonly static byte[] _pngSignature = new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a };
        private readonly static byte[] _jpgSignature = new byte[] { 0xff, 0xd8, 0xff };
        private readonly static byte[] _ddsSignature = new byte[] { 0x44, 0x44, 0x53, 0x20 };

        /// <summary>
        /// Determines if the given data is a TGA file.
        /// </summary>
        private static bool IsTga(byte[] data)
        {
            // TGA doesn't have a signature, the best way is to read the header and check if the values make sense.
            BinaryReader reader = new BinaryReader(new MemoryStream(data));

            // First field - image ID length, 1 byte.
            reader.ReadByte();   // ignore

            // Second field - color map type, 1 byte, must be 0 or 1.
            byte colorMapType = reader.ReadByte();
            if (!(colorMapType == 0 || colorMapType == 1))
            {
                return false;
            }

            // Third field - image type, 1 byte, must be in ranges 0-3 or 9-11
            byte imageType = reader.ReadByte();
            if ((imageType > 3 && imageType < 9) || imageType > 11)
            {
                return false;
            }

            // Fourth field - color map, 5 bytes
            reader.ReadUInt16();    // ignore
            reader.ReadUInt16();    // ignore
            byte colorSize = reader.ReadByte();
            if (!(colorSize == 15 || colorSize == 16 || colorSize == 24 || colorSize == 32))
            {
                return false;
            }

            // Fifth field - image specification, 10 bytes
            reader.ReadUInt16();    // ignore
            reader.ReadUInt16();    // ignore
            reader.ReadUInt16();    // ignore
            reader.ReadUInt16();    // ignore
            byte pixelDepth = reader.ReadByte();
            if (!(pixelDepth == 15 || pixelDepth == 16 || pixelDepth == 24 || pixelDepth == 32))
            {
                return false;
            }
            byte imageDescriptor = reader.ReadByte();
            if ((imageDescriptor & 192) != 0)
            {
                return false;
            }

            // Static header ends here and variable length fields begin. If execution reaches this point, the header *probably* contains valid data.
            reader.Close();
            return true;
        }

        public static ImageFileFormat DetectFormat(byte[] data, string extension)
        {
            // Detect PNG
            if (extension == "png" && data.Take(8).SequenceEqual(_pngSignature))
            {
                return ImageFileFormat.Png;
            }
            else if (data.Take(3).SequenceEqual(_jpgSignature))
            {
                return ImageFileFormat.Jpg;
            }
            else if (extension == "dds" && data.Take(4).SequenceEqual(_ddsSignature))
            {
                return ImageFileFormat.Dds;
            }
            else if (extension == "tga" && IsTga(data))
            {
                return ImageFileFormat.Tga;
            }

            return ImageFileFormat.Unsupported;
        }

        public static bool IsTranslucent(this Texture2D tex, float threshold = 1.0f)
        {
            int w = tex.width;
            int h = tex.height;
            for(int y = 0; y < h; ++y)
            {
                for(int x = 0; x < w; ++x)
                {
                    if (tex.GetPixel(x, y).a < threshold)
                        return true;
                }
            }
            return false;
        }
    }

    public static class PmxConvert
    {
        public static float Scale { get; } = 0.33333f;

        /// <summary>
        /// Loads a texture from the specified file path.
        /// </summary>
        public static Texture2D LoadTexture(string path)
        {
            return LoadTexture(File.ReadAllBytes(path));
        }

        /// <summary>
        /// Loads a texture from the provided data.
        /// </summary>
        public static Texture2D LoadTexture(byte[] data)
        {
            // Check the file format
            // TODO

            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(data);
            return tex;
        }

        /// <summary>
        /// Converts a PMX model to a Unity object.
        /// </summary>
        public static GameObject Load(this PmxModel model, GameObject bonePrefab)
        {
            // Model root
            GameObject root = new GameObject(model.NameJapanese);
            SceneObject component = root.AddComponent<SceneObject>();

            component.NameEnglish = model.NameEnglish;
            component.NameJapanese = model.NameJapanese;
            component.DescriptionEnglish = model.DescriptionEnglish;
            component.DescriptionJapanese = model.DescriptionJapanese;
            component.FilePath = model.FilePath;

            #region Skeleton
            Transform skeletonRoot = new GameObject("Skeleton").transform;
            skeletonRoot.SetParent(root.transform);
            Transform[] bones = new Transform[model.Bones.Length];

            // First pass - create bones and copy properties
            PmxBoneBehaviour leftShoulder = null;
            PmxBoneBehaviour rightShoulder = null;
            for (int i = 0; i < bones.Length; ++i)
            {
                // Create bone
                PmxBone original = model.Bones[i];
                GameObject bone = GameObject.Instantiate<GameObject>(bonePrefab);
                PmxBoneBehaviour c = bone.GetComponent<PmxBoneBehaviour>();
                Transform t = bone.transform;

                // Copy properties
                c.Name = original.Name;
                bone.name = original.Name;
                t.position = original.Position * Scale;
                c.Interactive = original.HasFlag(PmxBoneFlags.Visible);

                c.Tail = original.HasFlag(PmxBoneFlags.TailIsIndex) ? PmxBoneBehaviour.TailType.Bone : PmxBoneBehaviour.TailType.Vector;
                if ((c.Tail == PmxBoneBehaviour.TailType.Vector && original.TailPosition.magnitude <= 0) || (c.Tail == PmxBoneBehaviour.TailType.Bone && original.TailIndex < 0))
                {
                    c.Tail = PmxBoneBehaviour.TailType.None;
                }

                if (c.Tail == PmxBoneBehaviour.TailType.Vector)
                    c.TailPosition = original.TailPosition * Scale;

                c.Flags = 0;
                if (original.HasFlag(PmxBoneFlags.Rotation))
                    c.Flags |= PmxBoneBehaviour.BoneFlags.Rotation;
                if (original.HasFlag(PmxBoneFlags.Translation))
                    c.Flags |= PmxBoneBehaviour.BoneFlags.Translation;
                if (original.HasFlag(PmxBoneFlags.Visible))
                    c.Flags |= PmxBoneBehaviour.BoneFlags.Visible;
                if (original.HasFlag(PmxBoneFlags.FixedAxis))
                    c.Flags |= PmxBoneBehaviour.BoneFlags.Twist;

                c.UpdateSprite();

                bones[i] = bone.transform;

                if (leftShoulder == null && c.Name == "左腕")
                {
                    //leftShoulder = model.Bones[i];
                    leftShoulder = c;
                    Log.Trace("Found left shoulder");
                }
                else if (rightShoulder == null && c.Name == "右腕")
                {
                    //rightShoulder = model.Bones[i];
                    rightShoulder = c;
                    Log.Trace("Found right shoulder");
                }
            }

            // Second pass - set up bone hierarchy and tails
            for (int i = 0; i < bones.Length; ++i)
            {
                PmxBone original = model.Bones[i];
                PmxBoneBehaviour bone = bones[i].GetComponent<PmxBoneBehaviour>();

                bone.transform.SetParent(original.ParentIndex < 0 ? skeletonRoot : bones[original.ParentIndex]);
                if (bone.Tail == PmxBoneBehaviour.TailType.Bone)
                {
                    bone.TailBone = bones[original.TailIndex];
                }
            }

            // Final pass - reorient arm and custom axis bones
            if (leftShoulder != null)
            {
                foreach (PmxBoneBehaviour bone in leftShoulder.GetComponentsInChildren<PmxBoneBehaviour>())
                {
                    bone.transform.Reorient(Quaternion.Euler(0, 0, 45) * Quaternion.Euler(0, 0, -90));
                }
            }
            if (rightShoulder != null)
            {
                foreach (PmxBoneBehaviour bone in rightShoulder.GetComponentsInChildren<PmxBoneBehaviour>())
                {
                    bone.transform.Reorient(Quaternion.Euler(0, 0, -45) * Quaternion.Euler(0, 0, -90));
                }
            }
            #endregion

            #region Mesh

            Transform meshRoot = new GameObject("Mesh").transform;
            meshRoot.SetParent(root.transform);

            Mesh[] meshes = new Mesh[model.Materials.Length];

            // Create the meshes
            for (int i = 0; i < meshes.Length; ++i)
            {
                // Read the mesh data
                PmxMaterial material = model.Materials[i];
                PmxSurface[] surfaces = material.GetSurfaces(model.Surfaces);
                List<PmxVertex> vertices = new List<PmxVertex>();
                List<int> triangles = new List<int>();
                foreach (PmxSurface s in material.GetSurfaces(model.Surfaces))
                {
                    vertices.Add(model.Vertices[s.Vertex0]);
                    triangles.Add(vertices.Count - 1);
                    vertices.Add(model.Vertices[s.Vertex1]);
                    triangles.Add(vertices.Count - 1);
                    vertices.Add(model.Vertices[s.Vertex2]);
                    triangles.Add(vertices.Count - 1);
                }

                // Create Unity mesh
                Mesh mesh = new Mesh();
                mesh.name = material.Name;
                mesh.vertices = PmxVertex.GetPositions(vertices, Scale);
                mesh.normals = PmxVertex.GetNormals(vertices);
                mesh.uv = PmxVertex.GetUVs(vertices);
                mesh.triangles = triangles.ToArray();
                mesh.boneWeights = PmxVertex.GetBoneWeights(PmxVertex.GetDeforms(vertices));

                meshes[i] = mesh;
            }

            #endregion
            #region Mesh Renderer

            // Create objects and components
            /*
            for (int i = 0; i < meshes.Length; ++i)
            {
                Mesh mesh = meshes[i];
                GameObject o = new GameObject(mesh.name);
                MeshFilter mf = o.AddComponent<MeshFilter>();
                mf.sharedMesh = mesh;
                MeshRenderer mr = o.AddComponent<MeshRenderer>();

                PmxMaterial pmxMat = model.Materials[i];
                Material mat = new Material(Shader.Find("Standard"));
                mr.sharedMaterial = mat;

                mat.color = pmxMat.Diffuse;
                if (pmxMat.TextureIndex >= 0)
                {
                    string path = Path.Combine(Path.GetDirectoryName(model.FilePath), model.TexturePaths[pmxMat.TextureIndex]);
                    if (File.Exists(path))
                    {
                        mat.mainTexture = LoadTexture(path);
                    }
                }

                o.transform.SetParent(meshRoot);
            }*/
            
            Texture2D[] textures = new Texture2D[model.TexturePaths.Length];
            for(int i = 0; i < textures.Length; ++i)
            {
                string path = Path.Combine(Path.GetDirectoryName(model.FilePath), model.TexturePaths[i]);
                if(File.Exists(path))
                {
                    textures[i] = LoadTexture(path);
                }
                else
                {
                    textures[i] = new Texture2D(2, 2);
                    textures[i].SetPixels(new Color[] { Color.magenta, Color.magenta, Color.magenta, Color.magenta });
                }
            }

            for (int i = 0; i < meshes.Length; ++i)
            {
                Mesh mesh = meshes[i];
                GameObject o = new GameObject(mesh.name);
                MeshFilter mf = o.AddComponent<MeshFilter>();
                mf.sharedMesh = mesh;
                SkinnedMeshRenderer smr = o.AddComponent<SkinnedMeshRenderer>();
                smr.sharedMesh = mesh;
                smr.updateWhenOffscreen = true;

                smr.rootBone = o.transform;
                smr.bones = bones;

                List<Matrix4x4> bp = new List<Matrix4x4>();
                foreach (Transform bone in bones)
                {
                    bp.Add(bone.worldToLocalMatrix);
                }
                mesh.bindposes = bp.ToArray();

                PmxMaterial pmxMat = model.Materials[i];
                Material mat = new Material(Shader.Find("Standard"));
                smr.sharedMaterial = mat;

                mat.color = pmxMat.Diffuse;
                if (pmxMat.TextureIndex >= 0)
                {
                    mat.mainTexture = textures[pmxMat.TextureIndex];
                }

                //foreach(MaterialDirective dir in pmxMat.Directives)
                //{
                //    dir.Execute(mat);
                //    dir.Execute(smr);
                //}

                o.transform.SetParent(meshRoot);
            }

            // Set up skinned mesh

            #endregion

            component.MeshRoot = meshRoot;
            component.SkeletonRoot = skeletonRoot;
            return root;
        }
    }
}
