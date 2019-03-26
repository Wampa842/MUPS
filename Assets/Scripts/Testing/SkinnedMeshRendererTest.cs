using System;
using System.Collections.Generic;
using UnityEngine;

namespace MUPS
{
    public static class Testing
    {
        public static GameObject CreateTestModel()
        {
            // Create container objects
            GameObject root = new GameObject("Skinned Mesh Test");
            PmxModel c = root.AddComponent<PmxModel>();
            c.DisplayName = "Skinned Mesh Test";
            GameObject mp = new GameObject("Model");
            mp.transform.SetParent(root.transform);
            GameObject bp = new GameObject("Skeleton");
            bp.transform.SetParent(root.transform);

            // Create mesh
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[]
            {
                new Vector3(-0.1f, 0, 0.1f),        // 0
                new Vector3(-0.1f, 0, -0.1f),       // 1
                new Vector3(0.1f, 0, -0.1f),        // 2
                new Vector3(0.1f, 0, 0.1f),         // 3

                new Vector3(-0.1f, 0.5f, 0.1f),     // 4
                new Vector3(-0.1f, 0.5f, -0.1f),    // 5
                new Vector3(0.1f, 0.5f, -0.1f),     // 6
                new Vector3(0.1f, 0.5f, 0.1f),      // 7

                new Vector3(-0.1f, 1, 0.1f),        // 8
                new Vector3(-0.1f, 1, -0.1f),       // 9
                new Vector3(0.1f, 1, -0.1f),        // 10
                new Vector3(0.1f, 1, 0.1f)          // 11
            };

            mesh.triangles = new int[]
            {
                // bottom
                2, 3, 0,
                0, 1, 2,

                // front
                6, 2, 1,
                1, 5, 6,
                // back
                3, 7, 4,
                3, 4, 0,
                // left
                5, 1, 0,
                0, 4, 5,
                // right
                3, 2, 6,
                6, 7, 3,

                // front
                10, 6, 5,
                5, 9, 10,
                // back
                7, 11, 8,
                7, 8, 4,
                // left
                9, 5, 4,
                4, 8, 9,
                // right
                7, 6, 10,
                10, 11, 7,

                // top
                10, 9, 8,
                8, 11, 10
            };

            mesh.RecalculateNormals();

            GameObject mc = new GameObject("cube");
            mc.transform.SetParent(mp.transform);
            mc.AddComponent<MeshFilter>().sharedMesh = mesh;
            SkinnedMeshRenderer smr = mc.AddComponent<SkinnedMeshRenderer>();
            smr.sharedMesh = mesh;
            smr.material = new Material(Shader.Find("Standard"));

            // Create bones
            GameObject original = Resources.Load<GameObject>("Prefabs/GUI/Bone");
            Transform b0 = GameObject.Instantiate<GameObject>(original).transform;
            Transform b1 = GameObject.Instantiate<GameObject>(original).transform;
            Transform b2 = GameObject.Instantiate<GameObject>(original).transform;
            b1.position = new Vector3(0, 0.5f, 0);
            b1.SetParent(b0);
            //b1.SetParent(bp.transform);
            b2.position = new Vector3(0, 1, 0);
            b2.SetParent(b1);
            //b2.SetParent(bp.transform);
            b0.SetParent(bp.transform);

            // Set up bone components

            PmxBone c0 = b0.GetComponent<PmxBone>();
            PmxBone c1 = b1.GetComponent<PmxBone>();
            PmxBone c2 = b2.GetComponent<PmxBone>();
            c0.Tail = PmxBone.TailType.Bone;
            c0.TailBone = b1;
            c1.Tail = PmxBone.TailType.Bone;
            c1.TailBone = b2;
            c2.Tail = PmxBone.TailType.Vector;
            c2.TailPosition = new Vector3(0, 0.5f, 0);

            // Set up skinned mesh
            smr.bones = new Transform[] { b0, b1, b2 };

            mesh.boneWeights = new BoneWeight[]
            {
                new BoneWeight{ boneIndex0 = 0, weight0 = 1 },
                new BoneWeight{ boneIndex0 = 0, weight0 = 1 },
                new BoneWeight{ boneIndex0 = 0, weight0 = 1 },
                new BoneWeight{ boneIndex0 = 0, weight0 = 1 },
                new BoneWeight{ boneIndex0 = 1, weight0 = 1 },
                new BoneWeight{ boneIndex0 = 1, weight0 = 1 },
                new BoneWeight{ boneIndex0 = 1, weight0 = 1 },
                new BoneWeight{ boneIndex0 = 1, weight0 = 1 },
                new BoneWeight{ boneIndex0 = 2, weight0 = 1 },
                new BoneWeight{ boneIndex0 = 2, weight0 = 1 },
                new BoneWeight{ boneIndex0 = 2, weight0 = 1 },
                new BoneWeight{ boneIndex0 = 2, weight0 = 1 }
            };

            mesh.bindposes = new Matrix4x4[]
            {
                b0.worldToLocalMatrix,
                b1.worldToLocalMatrix,
                b2.worldToLocalMatrix
            };

            return root;
        }
    }
}