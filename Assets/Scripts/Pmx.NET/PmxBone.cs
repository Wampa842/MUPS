using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PmxSharp
{
    [Flags]
    public enum PmxBoneFlags { TailIsIndex = 1, Rotation = 2, Translation = 4, Visible = 8, Enabled = 16, IK = 32, InheritRotation = 256, InheritTranslation = 512, FixedAxis = 1024, LocalCoordinateSystem = 2048, PhysicsAfterDeform = 4096, ExternalParentDeform = 8192 }

    public class PmxBone : PmxItem
    {
        public int Index { get; set; }
        public PmxBoneFlags Flags { get; set; }
        public Vector3 Position { get; set; }
        public int ParentIndex { get; set; }
        public int DeformOrder { get; set; }
        public Vector3 TailPosition { get; set; }
        public int TailIndex { get; set; }
        public int InheritBone { get; set; }
        public float InheritMultiplier { get; set; }
        public Vector3 FixedAxis { get; set; }
        public Vector3 LocalCoordinateX { get; set; }
        public Vector3 LocalCoordinateZ { get; set; }
        public int ExternalParent { get; set; }
        public PmxIK IK { get; set; }

        public PmxBone ParentBone { get; set; }
        public List<PmxBone> Children { get; set; }

        /// <summary>
        /// Returns true if any of the specified flag bits are set.
        /// </summary>
        public bool HasFlag(PmxBoneFlags flag)
        {
            return (Flags & flag) != 0;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Bone {0}\n", Name)
                .AppendFormat("Parent {0}\n", ParentIndex)
                .AppendFormat("Position {0}\n", Position)
                .Append(HasFlag(PmxBoneFlags.IK) ? "IK" : "Not IK");
            return sb.ToString();
        }
    }
}
