using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MUPS
{
    /// <summary>
    /// Represents the root element of a PMX model.
    /// </summary>
    class PmxModel : MonoBehaviour
    {
        public static bool PreferJapaneseText = false;

        // UI
        public Button ListButton = null;
        public bool Ignore = false;
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

        public Transform[] RootBones = null;

        public bool BonesVisible
        {
            get { return BonesVisible; }
            set
            {
                foreach (Transform bone in RootBones)
                {
                    SetBoneVisible(bone, value);
                }
                BonesVisible = value;
            }
        }

        private static void SetBoneVisible(Transform bone, bool visible)
        {
            for (int i = 0; i < bone.childCount; ++i)
            {
                SetBoneVisible(bone.GetChild(i), visible);
            }
            foreach (Renderer r in bone.GetComponents<Renderer>())
                r.enabled = visible;
        }
    }
}
