using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MUPS.UI
{
    public class ModelInfoController : MonoBehaviour
    {
        public static ModelInfoController Instance { get; private set; }

        [Header("Prefabs")]
        public GameObject MorphSliderPrefab;
        public GameObject BoneButtonPrefab;

        [Header("Details")]
        public Text NameLabelEn;
        public Text NameLabelJa;
        public Text TypeLabel;

        [Header("Skeleton")]
        public RectTransform BoneList;
        public Toggle ShowInvisible;

        [Header("Morphs")]
        public RectTransform MorphList;
        public RectTransform EyesList;
        public RectTransform EyebrowList;
        public RectTransform MouthList;
        public RectTransform OtherList;

        public void ReadModelInfo(SceneObject obj)
        {
            ReadDetails(obj);
            ReadMorphs(obj);
            ReadBones(obj);
        }

        public void ReadDetails(SceneObject obj)
        {
            if (obj == null)
            {
                NameLabelEn.text = NameLabelJa.text = TypeLabel.text = "(nothing)";
                return;
            }

            NameLabelEn.text = obj.NameEnglish;
            NameLabelJa.text = obj.NameJapanese;

            if (obj is SceneModel)
                TypeLabel.text = "Model";
            else if (obj is SceneLight)
                TypeLabel.text = "Light";
            else
                TypeLabel.text = "Object";

            // TODO
        }

        public void ReadMorphs(SceneObject obj)
        {
            if (obj == null)
            {
                SceneModel.HideAllMorphLists();
                return;
            }

            if (obj is SceneModel)
            {
                SceneModel model = (SceneModel)obj;

                foreach (VertexMorph morph in model.VertexMorphs)
                {
                    GameObject s = Instantiate<GameObject>(MorphSliderPrefab);
                    MorphSlider c = s.GetComponent<MorphSlider>();

                    c.Label.text = morph.Name;
                    c.Slider.value = 0;
                    c.Morph = morph;
                    c.Owner = model;

                    switch (morph.Panel)
                    {
                        case PmxSharp.PmxMorphPanel.Eye:
                            s.transform.SetParent(EyesList);
                            break;
                        case PmxSharp.PmxMorphPanel.Eyebrow:
                            s.transform.SetParent(EyebrowList);
                            break;
                        case PmxSharp.PmxMorphPanel.Mouth:
                            s.transform.SetParent(MouthList);
                            break;
                        case PmxSharp.PmxMorphPanel.Other:
                            s.transform.SetParent(OtherList);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void ReadBones(SceneObject obj)
        {
            if (obj == null)
            {
                SceneObject.HideAllBoneButtons();
                return;
            }

            PmxBoneBehaviour[] bones = obj.SkeletonRoot.GetComponentsInChildren<PmxBoneBehaviour>();
            Array.Sort<PmxBoneBehaviour>(bones, (a, b) => a.Index.CompareTo(b.Index));
            obj.BoneButtons = new BoneButton[bones.Length];
            for (int i = 0; i < bones.Length; ++i)
            {
                PmxBoneBehaviour bone = bones[i];
                BoneButton button = Instantiate<GameObject>(BoneButtonPrefab).GetComponent<BoneButton>();
                button.Bone = bone;
                button.Owner = obj;
                button.GetComponentInChildren<Text>().text = bone.Name;
                button.transform.SetParent(BoneList);
                button.gameObject.SetActive(bone.HasFlag(PmxBoneBehaviour.BoneFlags.Visible));
                obj.BoneButtons[i] = button;
            }
        }

        public void UpdateBoneVisibility(bool visible)
        {
            SceneController.Instance.SelectedModel?.ShowBoneButtons();
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(Instance);
                Instance = this;
            }
        }
    }
}
