using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using MUPS;

namespace PmxSharp
{
    public static class PmxConvert
    {
        public static Texture2D LoadTexture(string path)
        {
            return LoadTexture(File.ReadAllBytes(path));
        }

        public static Texture2D LoadTexture(byte[] data)
        {
            // Check the file format
            // TODO

            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(data);
            return tex;
        }

        public static GameObject Load(this PmxModel model, GameObject bonePrefab)
        {
            // Model root
            GameObject root = new GameObject(model.NameJapanese);
            PmxModelBehaviour component = root.AddComponent<PmxModelBehaviour>();

            component.NameEnglish = model.NameEnglish;
            component.FileName = model.NameJapanese;
            component.DescriptionEnglish = model.DescriptionEnglish;
            component.DescriptionJapanese = model.DescriptionJapanese;
            component.FilePath = model.FilePath;

            #region Skeleton
            Transform skeletonRoot = new GameObject("Skeleton").transform;
            skeletonRoot.SetParent(root.transform);
            Transform[] bones = new Transform[model.Bones.Length];

            // First pass - create bones and copy properties
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
                t.position = original.Position;
                c.Interactive = original.HasFlag(PmxBoneFlags.Visible);

                c.Tail = original.HasFlag(PmxBoneFlags.TailIsIndex) ? PmxBoneBehaviour.TailType.Bone : PmxBoneBehaviour.TailType.Vector;
                if ((c.Tail == PmxBoneBehaviour.TailType.Vector && original.TailPosition.magnitude <= 0) || (c.Tail == PmxBoneBehaviour.TailType.Bone && original.TailIndex < 0))
                {
                    c.Tail = PmxBoneBehaviour.TailType.None;
                }

                if (c.Tail == PmxBoneBehaviour.TailType.Vector)
                    c.TailPosition = original.TailPosition;

                c.Flags = 0;
                if (original.HasFlag(PmxBoneFlags.Translation))
                    c.Flags |= PmxBoneBehaviour.BoneFlags.Translation;
                if (original.HasFlag(PmxBoneFlags.Visible))
                    c.Flags |= PmxBoneBehaviour.BoneFlags.Visible;
                if (original.HasFlag(PmxBoneFlags.FixedAxis))
                    c.Flags |= PmxBoneBehaviour.BoneFlags.Twist;

                c.UpdateSprite();

                bones[i] = bone.transform;
            }

            // Second pass - set up bone hierarchy and tails
            for (int i = 0; i < bones.Length; ++i)
            {
                PmxBone original = model.Bones[i];
                PmxBoneBehaviour bone = bones[i].GetComponent<PmxBoneBehaviour>();
                bone.transform.SetParent(original.Parent < 0 ? skeletonRoot : bones[original.Parent]);
                if (bone.Tail == PmxBoneBehaviour.TailType.Bone)
                {
                    bone.TailBone = bones[original.TailIndex];
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
                mesh.vertices = PmxVertex.GetPositions(vertices);
                mesh.normals = PmxVertex.GetNormals(vertices);
                mesh.uv = PmxVertex.GetUVs(vertices);
                mesh.triangles = triangles.ToArray();
                mesh.boneWeights = PmxVertex.GetBoneWeights(PmxVertex.GetDeforms(vertices));

                meshes[i] = mesh;
            }

            // Create objects and components

            /*for (int i = 0; i < meshes.Length; ++i)
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

            for (int i = 0; i < meshes.Length; ++i)
            {
                Mesh mesh = meshes[i];
                GameObject o = new GameObject(mesh.name);
                MeshFilter mf = o.AddComponent<MeshFilter>();
                mf.sharedMesh = mesh;
                SkinnedMeshRenderer smr = o.AddComponent<SkinnedMeshRenderer>();
                smr.sharedMesh = mesh;

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
                    string path = Path.Combine(Path.GetDirectoryName(model.FilePath), model.TexturePaths[pmxMat.TextureIndex]);
                    if (File.Exists(path))
                    {
                        mat.mainTexture = LoadTexture(path);
                    }
                }

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
