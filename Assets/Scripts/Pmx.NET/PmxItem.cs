namespace PmxSharp
{
    /// <summary>
    /// An item that has both an English and a Japanese name.
    /// </summary>
    public abstract class PmxItem
    {
        /// <summary>
        /// Indicates whether local (Japanese) name should be preferred.
        /// </summary>
        public static bool PreferJapanese
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// The universal (English) name.
        /// </summary>
        public string NameEnglish { get; set; }

        /// <summary>
        /// The local (Japanese) name, usually used as a primary key.
        /// </summary>
        public string NameJapanese { get; set; }

        /// <summary>
        /// Returns the preferred name, or sets both names.
        /// </summary>
        public string Name
        {
            get
            {
                if (PreferJapanese)
                    return string.IsNullOrEmpty(NameJapanese) ? NameEnglish : NameJapanese;
                else
                    return string.IsNullOrEmpty(NameEnglish) ? NameJapanese : NameEnglish;
            }
            set
            {
                NameEnglish = NameJapanese = value;
            }
        }
    }
}
