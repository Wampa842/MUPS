using UnityEngine;

namespace PmxSharp
{
    public enum PmxJointType { SixDofSpring = 0, SixDof = 1, BallSocket = 2, ConeTwist = 3, Slider = 4, Hinge = 5 }

    public class PmxJoint : PmxItem
    {
        public PmxJointType Type { get; set; }
        public int Rigidbody0 { get; set; }
        public int Rigidbody1 { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 TranslationMin { get; set; }
        public Vector3 TranslationMax { get; set; }
        public Vector3 RotationMin { get; set; }
        public Vector3 RotationMax { get; set; }
        public Vector3 SpringTranslation { get; set; }
        public Vector3 SpringRotation { get; set; }
    }
}
