namespace PmxSharp
{
    public static class PmxIndex
    {
        public static int Vertex { get; set; }
        public static int Texture { get; set; }
        public static int Material { get; set; }
        public static int Bone { get; set; }
        public static int Morph { get; set; }
        public static int Rigidbody { get; set; }

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
