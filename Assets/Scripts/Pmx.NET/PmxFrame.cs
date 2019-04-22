using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmxSharp
{
    public enum PmxFrameType { Bone, Morph }

    public struct PmxFrameData
    {
        public PmxFrameType Type { get; set; }
        public int Index { get; set; }
    }

    public class PmxFrameGroup : PmxItem
    {
        public bool Special { get; set; }
        public PmxFrameData[] Frames { get; set; }
    }
}
