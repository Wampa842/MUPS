using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MUPS
{
    /// <summary>
    /// Represents the root element of a PMX model.
    /// </summary>
    class PmxModelBehaviour : MonoBehaviour
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

        public void SetBonesVisible(bool visible)
        {
            //TraverseBones(SkeletonRoot, (t, b) => { /*b.Interactive = visible;*/ Debug.Log(b.transform.name); });
            foreach (PmxBoneBehaviour bone in GetComponentsInChildren<PmxBoneBehaviour>())
                bone.Interactive = visible;
        }

        public void TraverseBones(Transform bone, Action<Transform, PmxBoneBehaviour> action)
        {
            PmxBoneBehaviour comp = bone.GetComponent<PmxBoneBehaviour>();
            if (comp != null)
                action.Invoke(bone, comp);

            for (int i = 0; i < bone.childCount; ++i)
            {
                Transform child = bone.GetChild(i);
                TraverseBones(child, action);
            }
        }
    }
}
