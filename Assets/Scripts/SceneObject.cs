using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MUPS
{
    /// <summary>
    /// Represents the root element of a PMX model.
    /// </summary>
    public class SceneObject : MonoBehaviour
    {
        public static bool PreferJapaneseText = false;

        // UI
        public Button ListButton = null;
        public bool Ignore = false;
        public string FilePath = "";
        public string NameEnglish = "none";
        public string NameJapanese = "none";
        public string DisplayName
        {
            get
            {
                if (PreferJapaneseText)
                    return string.IsNullOrEmpty(NameJapanese) ? NameEnglish : NameJapanese;
                else
                    return string.IsNullOrEmpty(NameEnglish) ? NameJapanese : NameEnglish;
            }
            set
            {
                NameEnglish = NameJapanese = value;
            }
        }
        public string DescriptionEnglish = "Empty description (English)";
        public string DescriptionJapanese = "Empty description (Japanese)";

        public Transform SkeletonRoot;
        public Transform MeshRoot;
        public PmxBoneBehaviour LastSelectedBone;

        public void SetBonesInteractive(bool interactive)
        {
            foreach (PmxBoneBehaviour bone in GetComponentsInChildren<PmxBoneBehaviour>())
                bone.Interactive = interactive;
        }

        public void TraverseBones(Transform bone, Action<Transform, PmxBoneBehaviour> action)
        {
            foreach(PmxBoneBehaviour b in GetComponentsInChildren<PmxBoneBehaviour>())
            {
                action.Invoke(b.transform, b);
            }
        }

        public static SceneObject Create(string name, bool rootBone = false)
        {
            GameObject root = new GameObject(name);
            SceneObject comp = root.AddComponent<SceneObject>();
            comp.SkeletonRoot = new GameObject("Skeleton").transform;
            comp.SkeletonRoot.SetParent(root.transform);
            comp.MeshRoot = new GameObject("Mesh").transform;
            comp.MeshRoot.SetParent(root.transform);
            if(rootBone)
            {
                GameObject bone = GameObject.Instantiate(SceneController.Instance.BonePrefab);
                bone.transform.SetParent(comp.SkeletonRoot);
                PmxBoneBehaviour bc = bone.GetComponent<PmxBoneBehaviour>();
                bone.name = bc.name = "root";
                bc.Interactive = true;
                bc.Flags = PmxBoneBehaviour.BoneFlags.Rotation | PmxBoneBehaviour.BoneFlags.Translation | PmxBoneBehaviour.BoneFlags.Visible;
                bc.Tail = PmxBoneBehaviour.TailType.None;
            }

            return comp;
        }
    }
}
