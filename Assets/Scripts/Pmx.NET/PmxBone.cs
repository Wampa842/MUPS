using System;
using System.Collections.Generic;
using UnityEngine;

namespace PmxSharp
{
    [Flags]
    public enum PmxBoneFlags { TailIsIndex = 1, Rotation = 2, Translation = 4, Visible = 8, Enabled = 16, IK = 32, InheritRotation = 256, InheritTranslation = 512, FixedAxis = 1024, LocalCoordinateSystem = 2048, PhysicsAfterDeform = 4096, ExternalParentDeform = 8192 }

    public class PmxBone : PmxItem
    {
        public PmxBoneFlags Flags { get; set; }
        public Vector3 Position { get; set; }
        public int Parent { get; set; }
        public int DeformationOrder { get; set; }
        public Vector3 TailPosition { get; set; }
        public int TailIndex { get; set; }
        public int InheritBone { get; set; }
        public float InheritMultiplier { get; set; }
        public Vector3 FixedAxis { get; set; }
        public Vector3 LocalCoordinateX { get; set; }
        public Vector3 LocalCoordinateZ { get; set; }
        public int ExternalParent { get; set; }
    }
}
