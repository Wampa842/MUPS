using UnityEngine;

namespace PmxSharp
{
    public enum PmxRigidbodyShape { Sphere = 0, Box = 1, Capsule = 2 }
    public enum PmxRigidbodyPhysics { Disabled = 0, Enabled = 1, Alternative = 2 }

    public class PmxRigidbody : PmxItem
    {
        public int BoneIndex { get; set; }
        public byte CollisionGroup { get; set; }
        public short CollisionIgnoreMask { get; set; }
        public PmxRigidbodyShape Shape { get; set; }
        public Vector3 Bounds { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public float Mass { get; set; }
        public float Drag { get; set; }
        public float RotationDrag { get; set; }
        public float Repulsion { get; set; }
        public float Friction { get; set; }
        public PmxRigidbodyPhysics PhysicsMode { get; set; }
    }
}
