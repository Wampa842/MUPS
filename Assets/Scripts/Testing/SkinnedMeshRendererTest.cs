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
            GameObject parent = new GameObject("Skinned Mesh Test");
            PmxModel c = parent.AddComponent<PmxModel>();
            c.DisplayName = "Skinned Mesh Test";
            GameObject mp = new GameObject("Model");
            mp.transform.SetParent(parent.transform);
            GameObject bp = new GameObject("Skeleton");
            bp.transform.SetParent(parent.transform);

            // Create mesh
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[]
            {
                new Vector3(-0.1f, 0, 0.1f),
                new Vector3(-0.1f, 0, -0.1f),
                new Vector3(0.1f, 0, -0.1f),
                new Vector3(0.1f, 0, 0.1f),
                new Vector3(-0.1f, 1, 0.1f),
                new Vector3(-0.1f, 1, -0.1f),
                new Vector3(0.1f, 1, -0.1f),
                new Vector3(0.1f, 1, 0.1f)
            };

            mesh.triangles = new int[]
            {
                // bottom
                2, 3, 0,
                0, 1, 2,
                // top
                6, 5, 4,
                4, 7, 6,
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
                6, 7, 3
            };

            mesh.RecalculateNormals();

            GameObject mc = new GameObject("cube");
            mc.AddComponent<MeshFilter>().sharedMesh = mesh;
            mc.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
            mc.transform.SetParent(mp.transform);

            // Create bones
            Transform b0 = new GameObject("bone0", typeof(PmxBone)).transform;
            Transform b1 = new GameObject("bone1", typeof(PmxBone)).transform;
            b1.position = new Vector3(0, 1, 0);
            b1.SetParent(b0);
            b0.SetParent(bp.transform);

            // Set up skinned mesh
            // TO-fcnk-DO

            return parent;
        }
    }
}