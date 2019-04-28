using UnityEngine;

namespace MUPS.Scene
{
    public struct BonePose
    {
        public PmxBoneBehaviour Bone { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        public BonePose(Vector3 position, Quaternion rotation)
        {
            Bone = null;
            Position = position;
            Rotation = rotation;
        }

        public BonePose(PmxBoneBehaviour bone)
        {
            Position = bone.transform.localPosition;
            Rotation = bone.transform.localRotation;
            Bone = bone;
        }

        public void Apply()
        {
            if(Bone != null)
            {
                Apply(Bone);
            }
        }

        public void Apply(PmxBoneBehaviour bone)
        {
            if(bone.HasFlag(PmxBoneBehaviour.BoneFlags.Translation))
            {
                bone.transform.localPosition = Position;
            }
            if(bone.HasFlag(PmxBoneBehaviour.BoneFlags.Rotation))
            {
                bone.transform.localRotation = Rotation;
            }
        }
    }
}
