using System;

namespace PmxSharp
{
    public class PmxModel : PmxItem
    {
        // File
        public string FilePath { get; set; }
        public PmxVersion Version { get; set; }
        public string DescriptionJapanese { get; set; }
        public string DescriptionEnglish { get; set; }

        // Globals
        public int AdditionalUVCount { get; set; }

        // Model data
        public PmxVertex[] Vertices { get; set; }
        public PmxSurface[] Surfaces { get; set; }
        public string[] TexturePaths { get; set; }
        public PmxMaterial[] Materials { get; set; }
        public PmxBone[] Bones { get; set; }


        #region Constructors
        public PmxModel()
        {
            Name = "New PMX Model";
        }

        public PmxModel(string name)
        {
            Name = name;
        }

        public PmxModel(string jp, string en)
        {
            NameJapanese = jp;
            NameEnglish = en;
        }
        #endregion
    }
}
