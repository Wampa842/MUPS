using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MUPS.SaveData;

namespace MUPS.UI
{
    class TransformControlBehaviour : MonoBehaviour
    {
        public static TransformControlBehaviour Instance { get; private set; }

        public Image RX, RY, RZ, TX, TY, TZ;
        public Transform CameraFacing;
        public GameObject ScreenGizmo;
        public Button ReferenceSystemButton;
        public bool Local = true;
        public bool ScreenGizmoVisible = false;

        public void CycleReferenceSystem()
        {
            Local = !Local;
            ReferenceSystemButton.GetComponentInChildren<Text>().text = Local ? "Local" : "Global";
        }

        public void SetTranslationEnabled(bool active)
        {
            if (active)
            {
                TX.color = Color.red;
                TY.color = Color.green;
                TZ.color = Color.blue;
            }
            else
            {
                TX.color = TY.color = TZ.color = Color.gray;
            }
        }

        public void Transform(int mode)
        {
            float delta = Input.GetAxis("Vertical") * ViewController.Instance.MovementMultiplier * 0.5f;
            Space space = Local ? Space.Self : Space.World;
            PmxBoneBehaviour bone = SceneController.Instance.SelectedBone;

            if (bone != null)
            {
                // Move selected object
                switch (mode)
                {
                    case 0:
                        if (bone.HasFlag(PmxBoneBehaviour.BoneFlags.Translation))
                            bone.transform.Translate(delta * 0.05f, 0, 0, space);
                        break;
                    case 1:
                        if (bone.HasFlag(PmxBoneBehaviour.BoneFlags.Translation))
                            bone.transform.Translate(0, delta * 0.05f, 0, space);
                        break;
                    case 2:
                        if (bone.HasFlag(PmxBoneBehaviour.BoneFlags.Translation))
                            bone.transform.Translate(0, 0, delta * 0.05f, space);
                        break;
                    case 3:
                        bone.transform.Rotate(delta, 0, 0, space);
                        break;
                    case 4:
                        bone.transform.Rotate(0, delta, 0, space);
                        break;
                    case 5:
                        bone.transform.Rotate(0, 0, delta, space);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Move camera
                switch (mode)
                {
                    case 0:
                        ViewController.Instance.Center.Translate(delta * 0.05f, 0, 0, Local ? ViewController.Instance.Pivot : ViewController.Instance.Center);
                        break;
                    case 1:
                        ViewController.Instance.Center.Translate(0, delta * 0.05f, 0, Local ? ViewController.Instance.Pivot : ViewController.Instance.Center);
                        break;
                    case 2:
                        ViewController.Instance.Center.Translate(0, 0, delta * 0.05f, Local ? ViewController.Instance.Pivot : ViewController.Instance.Center);
                        break;
                    case 3:
                        ViewController.Instance.Pivot.Rotate(delta, 0, 0, space);
                        break;
                    case 4:
                        ViewController.Instance.Pivot.Rotate(0, delta, 0, Space.World);
                        break;
                    case 5:
                        if (Local)
                            ViewController.Instance.Slider.Rotate(0, 0, delta, Space.Self);
                        else
                            ViewController.Instance.Pivot.Rotate(0, 0, delta, Space.World);
                        break;
                }
            }
        }

        public void ToggleScreenGizmo()
        {
            ScreenGizmoVisible = !ScreenGizmoVisible;
            ScreenGizmo.SetActive(ScreenGizmoVisible);
        }

        public void SetScreenGizmo(bool active)
        {
            ScreenGizmoVisible = active;
            ScreenGizmo.SetActive(ScreenGizmoVisible);
        }

        public void TransformScreen(int mode)
        {
            Debug.Log(mode);
            switch (mode)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                default:
                    break;
            }
        }

        public void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else if(Instance != this)
            {
                Destroy(Instance);
                Instance = this;
            }
        }

        private void Start()
        {
            SetScreenGizmo(false);
            Local = true;
            ReferenceSystemButton.GetComponentInChildren<Text>().text = "Local";
        }

        public void Update()
        {
            if(Settings.Current.Keyboard.ToggleScreenGizmo.Down())
            {
                ToggleScreenGizmo();
            }
            if(Settings.Current.Keyboard.ToggleLocal.Down())
            {
                CycleReferenceSystem();
            }

            ScreenGizmo.transform.rotation = CameraFacing.rotation;
            float dist = Camera.main.transform.InverseTransformPoint(ScreenGizmo.transform.position).z;
            float scale = dist * 0.15f;
            ScreenGizmo.transform.localScale = new Vector3(scale, scale, scale);
            //ScreenGizmo.transform.LookAt(Camera.main.transform, Camera.main.transform.up);
        }
    }
}
