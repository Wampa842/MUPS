using System;
using System.IO;

namespace PmxSharp
{
    /// <summary>
    /// A collection of types and sizes used by PMX.
    /// </summary>
    public static class PmxTypes
    {
        #region C types
        public const int Byte = 1;
        public const int SByte = 1;
        public const int Short = 2;
        public const int UShort = 2;
        public const int Int = 4;
        public const int UInt = 4;
        public const int Float = 4;
        #endregion
        #region Index types
        public enum IndexType { Vertex, Texture, Material, Bone, Morph, Rigidbody }
        public static int VertexIndex = 1;
        public static int TextureIndex = 1;
        public static int MaterialIndex = 1;
        public static int BoneIndex = 1;
        public static int MorphIndex = 1;
        public static int RigidbodyIndex = 1;
        public static int IndexSize(IndexType type)
        {
            switch (type)
            {
                case IndexType.Vertex:
                    return VertexIndex;
                case IndexType.Texture:
                    return TextureIndex;
                case IndexType.Material:
                    return MaterialIndex;
                case IndexType.Bone:
                    return BoneIndex;
                case IndexType.Morph:
                    return MorphIndex;
                case IndexType.Rigidbody:
                    return RigidbodyIndex;
                default:
                    return 1;
            }
        }
        #endregion
    }
}
