using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Crosstales.FB;
using MUPS.UI;
using PmxSharp;

namespace MUPS
{
    class SceneController : MonoBehaviour
    {
        // Singleton
        public static SceneController Instance { get; private set; }

        // References
        public Transform ModelListContent = null;                       // Container panel that holds the model list buttons
        public Button ToggleLocalButton = null;                         // Button that cycles the reference coordinate system
        private Button _cameraButton = null;                            // Model list button that selects "nothing" in the scene
        public Transform AxisGizmo = null;                              // Axis marker gizmo

        // Prefabs
        private GameObject _modelListButton;                    // Base model list button
        private GameObject _testModel;                          // Simple model for quick testing purposes
        private GameObject _bonePrefab;                         // Bone object prefab

        public bool Local = true;                              // Reference coordinate system
        public List<PmxModelBehaviour> SceneModels = null;     // List of models in the scene
        public PmxModelBehaviour SelectedModel = null;
        public PmxBoneBehaviour SelectedBone = null;

        #region Scene management
        public void FindModels()
        {
            foreach (PmxModelBehaviour m in GetComponentsInChildren<PmxModelBehaviour>())
                SceneModels.Add(m);

            PopulateModelList();
        }

        public void PopulateModelList()
        {
            for (int i = 0; i < ModelListContent.childCount; ++i)
            {
                Destroy(ModelListContent.GetChild(i).gameObject);
            }

            GameObject button = Instantiate<GameObject>(_modelListButton);
            _cameraButton = button.GetComponentInChildren<Button>();
            _cameraButton.transform.Find("SelectedIcon").GetComponent<Text>().enabled = false;
            button.GetComponentInChildren<Text>().text = "--- camera ---";
            button.transform.SetParent(ModelListContent);
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectModel(null);
            });

            foreach (PmxModelBehaviour m in SceneModels)
            {
                button = Instantiate<GameObject>(_modelListButton);
                button.GetComponentInChildren<Text>().text = m.DisplayName;
                button.transform.SetParent(ModelListContent);
                Button b = button.GetComponent<Button>();
                b.onClick.AddListener(() =>
                {
                    SelectModel(m);
                });
                b.transform.Find("SelectedIcon").GetComponent<Text>().enabled = false;
                m.ListButton = b;
            }

            if (SelectedModel == null)
            {
                _cameraButton.transform.Find("SelectedIcon").GetComponent<Text>().enabled = true;
            }
            else
            {
                SelectedModel.ListButton.transform.Find("SelectedIcon").GetComponent<Text>().enabled = true;
            }
        }

        public void AddTestModel()
        {
            //GameObject model = Instantiate<GameObject>(_testModel);
            GameObject model = Testing.CreateTestModel();
            Color c = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1);
            model.GetComponentInChildren<MeshRenderer>().material.color = c;
            PmxModelBehaviour comp = model.GetComponent<PmxModelBehaviour>();
            comp.DisplayName = model.name = string.Format(CultureInfo.InvariantCulture, "Test Model ({0:0.0} {1:0.0} {2:0.0})", c.r, c.g, c.b);
            model.transform.SetParent(transform);
            SceneModels.Add(comp);
            Logger.Log("Added " + comp.DisplayName);

            PopulateModelList();
            SelectModel(comp);
        }

        public void AddModel()
        {
            string path = FileBrowser.OpenSingleFile("Open model", /*Application.persistentDataPath*/ @"H:\MMD\UserFile\Model\004_Ruby", new ExtensionFilter("PMX files", "pmx"));
            if (!string.IsNullOrEmpty(path))
            {
                PmxImporter import = new PmxImporter(path);
                GameObject model = import.Import().Load(_bonePrefab);
                model.transform.SetParent(transform);
                PmxModelBehaviour comp = model.GetComponent<PmxModelBehaviour>();
                foreach (Transform bone in comp.SkeletonRoot)
                {
                    PmxBoneBehaviour b = bone.GetComponent<PmxBoneBehaviour>();
                    if (b != null)
                    {
                        comp.LastSelectedBone = b;
                        break;
                    }
                }

                SceneModels.Add(comp);
                Logger.Log("Added \"" + comp.DisplayName + "\"");
                PopulateModelList();
                SelectModel(comp);
            }
        }

        public void SelectModel(PmxModelBehaviour model)
        {
            foreach (Button b in ModelListContent.GetComponentsInChildren<Button>())
            {
                b.transform.Find("SelectedIcon").GetComponent<Text>().enabled = false;
            }

            foreach (PmxModelBehaviour comp in SceneModels)
            {
                comp.SetBonesInteractive(false);
            }

            if (model == null)
            {
                Logger.Log($"Selected camera");
                SelectedModel = null;
                _cameraButton.transform.Find("SelectedIcon").GetComponent<Text>().enabled = true;
                AxisGizmo.SetParent(transform);
                AxisGizmo.gameObject.SetActive(false);
            }
            else
            {
                Logger.Log($"Selected {model.DisplayName}");
                SelectedModel = model;
                SelectBone(model.LastSelectedBone);
                SelectedModel.ListButton.transform.Find("SelectedIcon").GetComponent<Text>().enabled = true;
                SelectedModel.SetBonesInteractive(true);
            }
        }

        public void DeleteSelected()
        {
            if (SelectedModel == null)
                return;

            SceneModels.Remove(SelectedModel);
            Destroy(SelectedModel.gameObject);
            PopulateModelList();
            SelectModel(null);
        }

        public void SelectBone(PmxBoneBehaviour bone)
        {
            SelectedBone = bone;
            SelectedModel.LastSelectedBone = SelectedBone;
            SelectedBone.SetColors();
            TransformControlBehaviour.Instance.SetTranslationEnabled(SelectedBone.HasFlag(PmxBoneBehaviour.BoneFlags.Translation));
            AxisGizmo.gameObject.SetActive(true);
            AxisGizmo.SetParent(SelectedBone.transform);
            AxisGizmo.localPosition = Vector3.zero;
            AxisGizmo.localRotation = Quaternion.identity;
            Logger.Log(string.Format("Selected bone {0}", SelectedBone.Name), Logger.LogLevel.Trace);
        }
        #endregion

        #region Scene settings
        public void CycleReferenceSystem()
        {
            Local = !Local;
            string label = Local ? "Local" : "Global";
            ToggleLocalButton.GetComponentInChildren<Text>().text = label;
            Logger.Log(label);
        }
        #endregion

        public void Awake()
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

            SaveData.Settings.Load();

            Logger.Log("### NEW SESSION (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ")");

            _modelListButton = Resources.Load<GameObject>("Prefabs/GUI/ModelListButton");
            _testModel = Resources.Load<GameObject>("Prefabs/TestModel");
            _bonePrefab = Resources.Load<GameObject>("Prefabs/GUI/Bone");
        }

        public void Start()
        {
            FindModels();
            SelectModel(null);
            ToggleLocalButton.GetComponentInChildren<Text>().text = Local ? "Local" : "Global";
        }

        public void Update()
        {
            float dist = Camera.main.transform.InverseTransformPoint(AxisGizmo.position).z;
            float scale = dist * SaveData.Settings.Current.View.AxisIndicatorSize;
            AxisGizmo.localScale = new Vector3(scale, scale, scale);
            if (Local)
            {
                AxisGizmo.localRotation = Quaternion.identity;
            }
            else
            {
                AxisGizmo.rotation = Quaternion.identity;
            }
        }

        public void OnApplicationQuit()
        {
            SaveData.Settings.Save();
        }
    }
}