using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using MUPS.UI;

namespace MUPS.Scene
{
    class SceneController : MonoBehaviour
    {
        // Singleton
        public static SceneController Instance;

        // References
        public Transform ModelListContent = null;
        private Button _cameraButton = null;

        // Prefabs
        private GameObject _modelListButton;
        private GameObject _testModel;

        public bool Local = false;
        public List<PmxModel> SceneModels = null;
        public PmxModel SelectedModel = null;
        public Transform SelectedItem = null;
        public Button ToggleLocalButton = null;

        public void FindModels()
        {
            foreach (PmxModel m in GetComponentsInChildren<PmxModel>())
                SceneModels.Add(m);

            PopulateModelList();
        }

        public void AddTestModel()
        {
            //GameObject model = Instantiate<GameObject>(_testModel);
            GameObject model = Testing.CreateTestModel();
            Color c = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1);
            model.GetComponentInChildren<MeshRenderer>().material.color = c;
            PmxModel comp = model.GetComponent<PmxModel>();
            comp.DisplayName = model.name = string.Format(CultureInfo.InvariantCulture, "Test Model ({0:0.0} {1:0.0} {2:0.0})", c.r, c.g, c.b);
            model.transform.SetParent(transform);
            SceneModels.Add(comp);
            Logger.Log("Added " + comp.DisplayName);

            PopulateModelList();
        }

        public void AddModel()
        {

        }

        public void SelectModel(PmxModel model)
        {
            foreach(Button b in ModelListContent.GetComponentsInChildren<Button>())
            {
                b.transform.Find("SelectedIcon").GetComponent<Text>().enabled = false;
            }

            if (model == null)
            {
                Logger.Log($"Selected camera");
                SelectedModel = null;
                SelectedItem = null;
                _cameraButton.transform.Find("SelectedIcon").GetComponent<Text>().enabled = true;
            }
            else
            {
                Logger.Log($"Selected {model.DisplayName}");
                SelectedModel = model;
                SelectedItem = model.transform;
                SelectedModel.ListButton.transform.Find("SelectedIcon").GetComponent<Text>().enabled = true;
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

            foreach (PmxModel m in SceneModels)
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

            if(SelectedModel == null)
            {
                _cameraButton.transform.Find("SelectedIcon").GetComponent<Text>().enabled = true;
            }
            else
            {
                SelectedModel.ListButton.transform.Find("SelectedIcon").GetComponent<Text>().enabled = true;
            }
        }

        public void CycleReferenceSystem()
        {
            Local = !Local;
            string label = Local ? "Local" : "Global";
            ToggleLocalButton.GetComponentInChildren<Text>().text = label;
            Logger.Log(label);
        }

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

            FindModels();
            SelectModel(null);
        }

        public void OnApplicationQuit()
        {
            SaveData.Settings.Save();
        }
    }
}