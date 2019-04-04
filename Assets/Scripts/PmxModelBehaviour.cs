using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MUPS
{
    /// <summary>
    /// Represents the root element of a PMX model.
    /// </summary>
    public class PmxModelBehaviour : MonoBehaviour
    {
        public static bool PreferJapaneseText = false;

        // UI
        public Button ListButton = null;
        public bool Ignore = false;
        public string FilePath = "";
        public string NameEnglish = "none";
        public string FileName = "none";
        public string DisplayName
        {
            get
            {
                if (PreferJapaneseText)
                    return string.IsNullOrEmpty(FileName) ? NameEnglish : FileName;
                else
                    return string.IsNullOrEmpty(NameEnglish) ? FileName : NameEnglish;
            }
            set
            {
                NameEnglish = FileName = value;
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
    }
}
