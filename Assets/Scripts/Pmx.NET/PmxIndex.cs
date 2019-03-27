namespace PmxSharp
{
    public enum PmxIndexType { Vertex, Texture, Material, Bone, Morph, Rigidbody }
    public static class PmxIndex
    {
        public static int Vertex { get; set; }
        public static int Texture { get; set; }
        public static int Material { get; set; }
        public static int Bone { get; set; }
        public static int Morph { get; set; }
        public static int Rigidbody { get; set; }
        public static int IndexSize(PmxIndexType type)
        {
            switch (type)
            {
                case PmxIndexType.Vertex:
                    return Vertex;
                case PmxIndexType.Texture:
                    return Texture;
                case PmxIndexType.Material:
                    return Material;
                case PmxIndexType.Bone:
                    return Bone;
                case PmxIndexType.Morph:
                    return Morph;
                case PmxIndexType.Rigidbody:
                    return Rigidbody;
                default:
                    throw new System.ArgumentException(string.Format("{0} is not a valid index. I honestly don't know how you did this.", type.ToString()));
            }
        }

        static PmxIndex()
        {
            Vertex = 4;
            Texture = 4;
            Material = 4;
            Bone = 4;
            Morph = 4;
            Rigidbody = 4;
        }
    }
}
