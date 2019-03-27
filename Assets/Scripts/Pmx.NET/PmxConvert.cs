using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MUPS;

namespace PmxSharp
{
    public static class PmxConvert
    {
        public static GameObject Load(this PmxModel model, GameObject bonePrefab)
        {
            // Model root
            GameObject root = new GameObject(model.NameJapanese);
            PmxModelBehaviour component = root.AddComponent<PmxModelBehaviour>();

            component.NameEnglish = model.NameEnglish;
            component.NameJapanese = model.NameJapanese;
            component.DescriptionEnglish = model.DescriptionEnglish;
            component.DescriptionJapanese = model.DescriptionJapanese;

            #region Skeleton
            Transform skeleton = new GameObject("Skeleton").transform;
            skeleton.SetParent(root.transform);
            Transform[] bones = new Transform[model.Bones.Length];

            // First pass - create bones and copy properties
            for (int i = 0; i < bones.Length; ++i)
            {
                // Create bone
                PmxBone original = model.Bones[i];
                GameObject bone = GameObject.Instantiate<GameObject>(bonePrefab);
                PmxBoneBehaviour c = bone.GetComponent<PmxBoneBehaviour>();
                Transform t = bone.transform;

                // Copy properties
                c.Name = original.Name;
                bone.name = original.Name;
                t.position = original.Position;
                c.Interactive = original.HasFlag(PmxBoneFlags.Visible);

                c.Tail = original.HasFlag(PmxBoneFlags.TailIsIndex) ? PmxBoneBehaviour.TailType.Bone : PmxBoneBehaviour.TailType.Vector;
                if ((c.Tail == PmxBoneBehaviour.TailType.Vector && original.TailPosition.magnitude <= 0) || (c.Tail == PmxBoneBehaviour.TailType.Bone && original.TailIndex < 0))
                {
                    c.Tail = PmxBoneBehaviour.TailType.None;
                }

                if (c.Tail == PmxBoneBehaviour.TailType.Vector)
                    c.TailPosition = original.TailPosition;

                bones[i] = bone.transform;
            }

            // Second pass - set up bone hierarchy and tails
            for (int i = 0; i < bones.Length; ++i)
            {
                PmxBone original = model.Bones[i];
                PmxBoneBehaviour bone = bones[i].GetComponent<PmxBoneBehaviour>();
                bone.transform.SetParent(original.Parent < 0 ? skeleton : bones[original.Parent]);
                if (bone.Tail == PmxBoneBehaviour.TailType.Bone)
                {
                    bone.TailBone = bones[original.TailIndex];
                }
            }
            #endregion



            return root;
        }
    }
}
