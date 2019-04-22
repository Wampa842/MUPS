using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace PmxSharp
{
    public abstract class PmxMorphOffset
    {
        public int Index { get; set; }
    }

    public class PmxGroupOffset : PmxMorphOffset
    {
        public float Influence { get; set; }
        public static PmxMorphOffset Read(BinaryReader reader)
        {
            PmxGroupOffset o = new PmxGroupOffset();
            o.Index = reader.ReadIndex(PmxIndexType.Morph);
            o.Influence = reader.ReadSingle();
            return o;
        }
    }

    public class PmxVertexOffset : PmxMorphOffset
    {
        public Vector3 Translation { get; set; }
        public static PmxMorphOffset Read(BinaryReader reader)
        {
            PmxVertexOffset o = new PmxVertexOffset();
            o.Index = reader.ReadIndex(PmxIndexType.Vertex);
            o.Translation = reader.ReadVector3();
            return o;
        }
    }

    public class PmxBoneOffset : PmxMorphOffset
    {
        public Vector3 Translation { get; set; }
        public Quaternion Rotation { get; set; }
        public static PmxMorphOffset Read(BinaryReader reader)
        {
            PmxBoneOffset o = new PmxBoneOffset();
            o.Index = reader.ReadIndex(PmxIndexType.Bone);
            o.Translation = reader.ReadVector3();
            o.Rotation = reader.ReadQuaternion();
            return o;
        }
    }

    public class PmxUVOffset : PmxMorphOffset
    {
        public Vector4 Transform { get; set; }
        public static PmxMorphOffset Read(BinaryReader reader)
        {
            PmxUVOffset o = new PmxUVOffset();
            o.Index = reader.ReadIndex(PmxIndexType.Vertex);
            o.Transform = reader.ReadVector4();
            return o;
        }
    }

    public class PmxMaterialOffset : PmxMorphOffset
    {
        public enum OffsetOperation { Add, Multiply }
        public OffsetOperation Operation { get; set; }
        public Color Diffuse { get; set; }
        public Color Specular { get; set; }
        public float Exponent { get; set; }
        public Color Ambient { get; set; }
        public Color Edge { get; set; }
        public float EdgeSize { get; set; }
        public Color TextureTint { get; set; }
        public Color SphereTint { get; set; }
        public Color ToonTint { get; set; }
        public static PmxMorphOffset Read(BinaryReader reader)
        {
            PmxMaterialOffset o = new PmxMaterialOffset();
            o.Index = reader.ReadIndex(PmxIndexType.Material);
            o.Operation = reader.ReadByte() == 0 ? OffsetOperation.Multiply : OffsetOperation.Add;
            o.Diffuse = reader.ReadColor4();
            o.Specular = reader.ReadColor3();
            o.Exponent = reader.ReadSingle();
            o.Ambient = reader.ReadColor3();
            o.Edge = reader.ReadColor4();
            o.EdgeSize = reader.ReadSingle();
            o.TextureTint = reader.ReadColor4();
            o.SphereTint = reader.ReadColor4();
            o.ToonTint = reader.ReadColor4();
            return o;
        }
    }

    public class PmxFlipOffset : PmxMorphOffset
    {
        public float Influence { get; set; }
        public static PmxMorphOffset Read(BinaryReader reader)
        {
            PmxFlipOffset o = new PmxFlipOffset();
            o.Index = reader.ReadIndex(PmxIndexType.Morph);
            o.Influence = reader.ReadSingle();
            return o;
        }
    }

    public class PmxImpulseOffset : PmxMorphOffset
    {
        public bool Local { get; set; }
        public Vector3 Acceleration { get; set; }
        public Vector3 Torque { get; set; }
        public static PmxMorphOffset Read(BinaryReader reader)
        {
            PmxImpulseOffset o = new PmxImpulseOffset();
            o.Index = reader.ReadIndex(PmxIndexType.Rigidbody);
            o.Local = reader.ReadByte() != 0;
            o.Acceleration = reader.ReadVector3();
            o.Torque = reader.ReadVector3();
            return o;
        }
    }

    public enum PmxMorphType { Group = 0, Vertex = 1, Bone = 2, UV = 3, UV1 = 4, UV2 = 5, UV3 = 6, UV4 = 7, Material = 8, Flip = 9, Impulse = 10 }
    public enum PmxMorphPanel { None = 0, Eyebrow = 1, Eye = 2, Mouth = 3, Other = 4 }

    public class PmxMorph : PmxItem
    {
        public PmxMorphType Type { get; set; }
        public PmxMorphPanel Panel { get; set; }
        public PmxMorphOffset[] Offsets { get; set; }
    }
}
