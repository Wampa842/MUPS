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

        [Header("References")]
        public Transform ModelListContent = null;                       // Container panel that holds the model list buttons
        public Button ToggleLocalButton = null;                         // Button that cycles the reference coordinate system
        private Button _cameraButton = null;                            // Model list button that selects "nothing" in the scene
        public Transform AxisGizmo = null;                              // Axis marker gizmo
        public List<SceneObject> SceneModels = null;     // List of models in the scene
        public SceneObject SelectedModel = null;
        public PmxBoneBehaviour SelectedBone = null;

        [Header("Prefabs")]
        private GameObject _modelListButton;                    // Base model list button
        private GameObject _modelListLightButton;               // Light object list button
        private GameObject _testModel;                          // Simple model for quick testing purposes
        public GameObject BonePrefab;                           // Bone object prefab

        #region Scene management
        public void FindModels()
        {
            foreach (SceneObject m in GetComponentsInChildren<SceneObject>())
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

            foreach (SceneObject m in SceneModels)
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
            SceneObject comp = model.GetComponent<SceneObject>();
            comp.DisplayName = model.name = string.Format(CultureInfo.InvariantCulture, "Test Model ({0:0.0} {1:0.0} {2:0.0})", c.r, c.g, c.b);
            model.transform.SetParent(transform);
            SceneModels.Add(comp);
            Log.Info("Added " + comp.DisplayName);

            PopulateModelList();
            SelectModel(comp);
        }

        public void AddModel()
        {
            string path = FileBrowser.OpenSingleFile("Open model", /*Application.persistentDataPath*/ @"H:\MMD\UserFile\Model\004_Ruby", new ExtensionFilter("PMX files", "pmx"));
            if (!string.IsNullOrEmpty(path))
            {
                PmxImporter import = new PmxImporter(path);
                GameObject model = import.Import().Load();
                model.transform.SetParent(transform);
                SceneObject comp = model.GetComponent<SceneObject>();
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
                Log.Info("Added \"" + comp.DisplayName + "\"");
                PopulateModelList();
                SelectModel(comp);
            }
        }

        public void AddLight()
        {
            SceneLight light = SceneLight.Create();
            SceneModels.Add(light);
            PopulateModelList();
            SelectModel(light);
            ModalController.Instance.LightColorPicker.Show(light);
        }

        public void SelectModel(SceneObject model)
        {
            foreach (Button b in ModelListContent.GetComponentsInChildren<Button>())
            {
                b.transform.Find("SelectedIcon").GetComponent<Text>().enabled = false;
            }

            foreach (SceneObject comp in SceneModels)
            {
                comp.SetBonesInteractive(false);
            }

            if (model == null)
            {
                Log.Trace($"Selected camera");
                SelectedModel = null;
                _cameraButton.transform.Find("SelectedIcon").GetComponent<Text>().enabled = true;
                AxisGizmo.SetParent(transform);
                AxisGizmo.gameObject.SetActive(false);
                SceneObject.HideAllBoneButtons();
                SceneModel.HideAllMorphLists();
            }
            else
            {
                Log.Trace($"Selected {model.DisplayName}");
                SelectedModel = model;
                SelectedModel.ListButton.transform.Find("SelectedIcon").GetComponent<Text>().enabled = true;
                SelectedModel.SetBonesInteractive(true);
                SelectBone(model.LastSelectedBone);
                SelectedModel.OnSelected.Invoke();
            }

            //ModelInfoController.Instance.ReadModelInfo(model);
        }

        public void EditSelected()
        {
            if(SelectedModel is SceneLight)
            {
                ModalController.Instance.LightColorPicker.Show(SelectedModel as SceneLight);
            }
        }

        public void DeleteSelected()
        {
            if (SelectedModel == null)
                return;
            TransformControlBehaviour.Instance.ScreenGizmo.transform.SetParent(transform);

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

            Transform screenGizmo = TransformControlBehaviour.Instance.ScreenGizmo.transform;
            screenGizmo.SetParent(SelectedBone.transform);
            screenGizmo.localPosition = Vector3.zero;

            Log.Trace(string.Format("Selected bone {0}", SelectedBone.Name));
        }
        
        public void ResetSelectedBone()
        {
            if(SelectedBone != null)
            {
                SelectedBone.ResetBone();
            }
        }
        #endregion

        //#region Scene settings
        //public void CycleReferenceSystem()
        //{
        //    Local = !Local;
        //    string label = Local ? "Local" : "Global";
        //    ToggleLocalButton.GetComponentInChildren<Text>().text = label;
        //    Log.Trace("Switched to reference system: " + label);
        //}
        //#endregion

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

            Log.WriteLog("### NEW SESSION (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ")");

            _modelListButton = Resources.Load<GameObject>("Prefabs/GUI/ModelListButton");
            _modelListLightButton = Resources.Load<GameObject>("Prefabs/GUI/ModelListLightButton");
            _testModel = Resources.Load<GameObject>("Prefabs/TestModel");
            BonePrefab = Resources.Load<GameObject>("Prefabs/GUI/Bone");
        }

        public void Start()
        {
            FindModels();
            SelectModel(null);
            //ToggleLocalButton.GetComponentInChildren<Text>().text = Local ? "Local" : "Global";
        }

        public void Update()
        {
            float dist = Camera.main.transform.InverseTransformPoint(AxisGizmo.position).z;
            float scale = dist * SaveData.Settings.Current.View.AxisIndicatorSize;
            AxisGizmo.localScale = new Vector3(scale, scale, scale);
            if (TransformControlBehaviour.Instance.Local)
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