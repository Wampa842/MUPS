using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MUPS
{
    /// <summary>
    /// Represents the root element of a scene object.
    /// </summary>
    public class SceneObject : MonoBehaviour
    {
        public static bool PreferJapaneseText = false;

        // UI
        public Button ListButton = null;
        public bool Ignore = false;
        public string FilePath = "";
        public string NameEnglish = "none123";
        public string NameJapanese = "none123";
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
                if (ListButton != null)
                {
                    ListButton.GetComponentInChildren<Text>().text = value;
                }
            }
        }

        public UnityEvent OnSelected { get; set; }

        public Transform SkeletonRoot;
        public Transform MeshRoot;
        public PmxBoneBehaviour LastSelectedBone;

        public UI.BoneButton[] BoneButtons { get; set; }

        public void HideBoneButtons()
        {
            foreach (UI.BoneButton button in BoneButtons)
            {
                button.gameObject.SetActive(false);
            }
        }

        public static void HideAllBoneButtons()
        {
            foreach (SceneObject o in SceneController.Instance.SceneModels)
            {
                o.HideBoneButtons();
            }
        }

        public void ShowBoneButtons()
        {
            SceneObject.HideAllBoneButtons();
            foreach (UI.BoneButton button in BoneButtons)
            {
                button.gameObject.SetActive(button.Bone.HasFlag(PmxBoneBehaviour.BoneFlags.Visible) || UI.ModelInfoController.Instance.ShowInvisible.isOn);
            }
        }

        public void SetBonesInteractive(bool interactive)
        {
            foreach (PmxBoneBehaviour bone in GetComponentsInChildren<PmxBoneBehaviour>())
                bone.Interactive = interactive;
        }

        public void TraverseBones(Transform bone, Action<Transform, PmxBoneBehaviour> action)
        {
            foreach (PmxBoneBehaviour b in GetComponentsInChildren<PmxBoneBehaviour>())
            {
                action.Invoke(b.transform, b);
            }
        }

        protected void Awake()
        {
            OnSelected = new UnityEvent();
        }

        protected virtual void Start()
        {
            OnSelected.AddListener(ShowBoneButtons);
        }

        public static SceneObject Create(string name, bool rootBone = false)
        {
            GameObject root = new GameObject("Object " + Resources.FindObjectsOfTypeAll<SceneObject>().Length);
            SceneObject comp = root.AddComponent<SceneObject>();
            comp.DisplayName = root.name;
            comp.SkeletonRoot = new GameObject("Skeleton").transform;
            comp.SkeletonRoot.SetParent(root.transform);
            comp.MeshRoot = new GameObject("Mesh").transform;
            comp.MeshRoot.SetParent(root.transform);
            GameObject bone = GameObject.Instantiate(SceneController.Instance.BonePrefab);
            bone.transform.SetParent(comp.SkeletonRoot);
            PmxBoneBehaviour bc = bone.GetComponent<PmxBoneBehaviour>();
            bone.name = bc.name = "root";
            bc.Interactive = true;
            bc.Flags = PmxBoneBehaviour.BoneFlags.Rotation | PmxBoneBehaviour.BoneFlags.Translation | PmxBoneBehaviour.BoneFlags.Visible;
            bc.Tail = PmxBoneBehaviour.TailType.Vector;
            bc.TailPosition = new Vector3(0, 0, 1);
            comp.LastSelectedBone = bc;

            root.transform.SetParent(SceneController.Instance.transform);
            return comp;
        }
    }
}
