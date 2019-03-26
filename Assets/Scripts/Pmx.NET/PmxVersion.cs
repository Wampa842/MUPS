using System;
using System.Linq;
using System.Collections.Generic;

namespace PmxSharp
{
    public enum PmxVersion { Unknown, Pmd, Pmx20, Pmx21 }

    /// <summary>
    /// PMX file header signatures
    /// </summary>
    public static class PmxVersionCheck
    {
        /// <summary>
        /// Header signature of legacy PMD files
        /// </summary>
        public static readonly byte[] PmdSignature = new byte[] { 0x50, 0x6d, 0x64 };
        /// <summary>
        /// Header signature of PMX 2.0 files
        /// </summary>
        public static readonly byte[] Pmx20Signature = new byte[] { 0x50, 0x4d, 0x58, 0x20, 0x00, 0x00, 0x00, 0x40 };
        /// <summary>
        /// Header signature of PMX 2.1 files
        /// </summary>
        public static readonly byte[] Pmx21Signature = new byte[] { 0x50, 0x4d, 0x58, 0x20, 0x66, 0x66, 0x06, 0x40 };

        /// <summary>
        /// Detects the file version from the first 8 bytes.
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public static PmxVersion Detect(byte[] header)
        {
            if (header.SequenceEqual(Pmx21Signature))
                return PmxVersion.Pmx21;
            if (header.SequenceEqual(Pmx20Signature))
                return PmxVersion.Pmx20;
            if (header.Take(3).SequenceEqual(PmdSignature))
                return PmxVersion.Pmd;
            return PmxVersion.Unknown;
        }

        public static bool FileIsPmx(byte[] sig)
        {
            return sig.SequenceEqual(Pmx20Signature.Take(4));
        }
    }
}
