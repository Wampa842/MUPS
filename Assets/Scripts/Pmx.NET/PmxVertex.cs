using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PmxSharp
{
    #region Weight types
    public interface IPmxDeform
    {
        /// <summary>
        /// Returns a BoneWeight based on the stored data.
        /// </summary>
        /// <returns></returns>
        BoneWeight GetBoneWeight();
    }

    /// <summary>
    /// Single-bone deform type.
    /// </summary>
    public class Bdef1Deform : IPmxDeform
    {
        public int Bone { get; set; }

        public BoneWeight GetBoneWeight()
        {
            return new BoneWeight
            {
                boneIndex0 = Bone,
                weight0 = 1.0f
            };
        }

        public Bdef1Deform()
        {
            Bone = 0;
        }

        public Bdef1Deform(int bone)
        {
            Bone = bone;
        }
    }

    /// <summary>
    /// Two-bone deform type.
    /// </summary>
    public class Bdef2Deform : IPmxDeform
    {
        public int Bone0 { get; set; }
        public int Bone1 { get; set; }
        public float Weight0 { get; set; }
        public float Weight1
        {
            get
            {
                return 1.0f - Weight0;
            }
            set
            {
                Weight0 = 1.0f - value;
            }
        }

        public BoneWeight GetBoneWeight()
        {
            return new BoneWeight
            {
                boneIndex0 = Bone0,
                boneIndex1 = Bone1,
                weight0 = Weight0,
                weight1 = Weight1
            };
        }
    }

    /// <summary>
    /// Four-bone deform type.
    /// </summary>
    public class Bdef4Deform : IPmxDeform
    {
        public int Bone0 { get; set; }
        public int Bone1 { get; set; }
        public int Bone2 { get; set; }
        public int Bone3 { get; set; }
        public float Weight0 { get; set; }
        public float Weight1 { get; set; }
        public float Weight2 { get; set; }
        public float Weight3 { get; set; }

        public BoneWeight GetBoneWeight()
        {
            return new BoneWeight
            {
                boneIndex0 = Bone0,
                boneIndex1 = Bone1,
                boneIndex2 = Bone2,
                boneIndex3 = Bone3,
                weight0 = Weight0,
                weight1 = Weight1,
                weight2 = Weight2,
                weight3 = Weight3
            };
        }
    }

    /// <summary>
    /// Four-bone dual quaternion deform type. Used the same way as BDEF4.
    /// </summary>
    public class QdefDeform : IPmxDeform
    {
        public int Bone0 { get; set; }
        public int Bone1 { get; set; }
        public int Bone2 { get; set; }
        public int Bone3 { get; set; }
        public float Weight0 { get; set; }
        public float Weight1 { get; set; }
        public float Weight2 { get; set; }
        public float Weight3 { get; set; }

        public BoneWeight GetBoneWeight()
        {
            return new BoneWeight
            {
                boneIndex0 = Bone0,
                boneIndex1 = Bone1,
                boneIndex2 = Bone2,
                boneIndex3 = Bone3,
                weight0 = Weight0,
                weight1 = Weight1,
                weight2 = Weight2,
                weight3 = Weight3
            };
        }
    }

    /// <summary>
    /// Two-bone spherical deform type. Used the same way as BDEF2.
    /// </summary>
    public class SdefDeform : IPmxDeform
    {
        public int Bone0 { get; set; }
        public int Bone1 { get; set; }
        public float Weight0 { get; set; }
        public float Weight1
        {
            get
            {
                return 1.0f - Weight0;
            }
            set
            {
                Weight0 = 1.0f - value;
            }
        }

        public Vector3 C { get; set; }
        public Vector3 R0 { get; set; }
        public Vector3 R1 { get; set; }

        public BoneWeight GetBoneWeight()
        {
            return new BoneWeight
            {
                boneIndex0 = Bone0,
                boneIndex1 = Bone1,
                weight0 = Weight0,
                weight1 = Weight1
            };
        }
    }
    #endregion

    public class PmxVertex
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Vector2 UV { get; set; }
        public Vector4[] AdditionalUVs { get; set; }
        public IPmxDeform Deform { get; set; }
        public float EdgeSize { get; set; }

        public PmxVertex()
        {
            Position = new Vector3();
            Normal = new Vector3(0, 1, 0);
            UV = new Vector2();
            AdditionalUVs = new Vector4[0];
            Deform = new Bdef1Deform(-1);
            EdgeSize = 1.0f;
        }

        public static Vector3[] GetPositions(IEnumerable<PmxVertex> coll, float scale = 1.0f)
        {
            List<Vector3> list = new List<Vector3>();
            foreach (var v in coll)
            {
                list.Add(v.Position * scale);
            }
            return list.ToArray();
        }

        public static Vector2[] GetUVs(IEnumerable<PmxVertex> coll)
        {
            List<Vector2> list = new List<Vector2>();
            foreach (var v in coll)
            {
                list.Add(v.UV);
            }
            return list.ToArray();
        }

        public static Vector3[] GetNormals(IEnumerable<PmxVertex> coll)
        {
            List<Vector3> list = new List<Vector3>();
            foreach (var v in coll)
            {
                list.Add(v.Normal);
            }
            return list.ToArray();
        }

        public static IPmxDeform[] GetDeforms(IEnumerable<PmxVertex> coll)
        {
            List<IPmxDeform> list = new List<IPmxDeform>();
            foreach(var v in coll)
            {
                list.Add(v.Deform);
            }
            return list.ToArray();
        }

        public static BoneWeight[] GetBoneWeights(IEnumerable<IPmxDeform> coll)
        {
            List<BoneWeight> list = new List<BoneWeight>();
            foreach (var def in coll)
            {
                list.Add(def.GetBoneWeight());
            }
            return list.ToArray();
        }
    }
}
