using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Crosstales.FB;
using MUPS.SaveData;
using MUPS.Scene;

namespace MUPS
{
    public class ViewController : MonoBehaviour
    {
        // Singleton instance
        public static ViewController Instance { get; private set; }

        // Objects
        public Transform Center;            // Camera center position, only used for panning
        public Transform Pivot;             // Camera orbit center, only rotated
        public Transform Slider;            // Camera distance from orbit center
        public Transform Gizmo;             // Camera center gizmo
        public GameObject FloorGrid;        // The origin floor grid
        public Camera GeometryCamera;       // Main geometry camera
        public Camera GUICamera;            // Camera that renders the user interface
        public Camera ScreenshotCamera;     // Camera for saving screenshots
        public Sprite RotateBoneSprite;     // Circular bone sprite
        public Sprite TranslateBoneSprite;  // Square bone sprite
        public Sprite TwistBoneSprite;      // X bone sprite
        public Transform FacingReverse;     // Transform that faces the camera

        // Inputs
        private bool _active;               // Control is active
        private float _scroll;              // Scroll wheel delta
        private bool _lmb;                  // Mouse 0
        private bool _rmb;                  // Mouse 1
        private bool _mmb;                  // Mouse 2
        private bool _shift;                // Left/right shift
        private bool _ctrl;                 // Left/right control
        private bool _alt;                  // Left alt

        private bool _bonesShown = true;

        // Global control access
        public float MovementMultiplier { get { return (_shift ? 3 : 1) * (_ctrl ? 0.33f : 1); } }

        public float FieldOfView
        {
            get { return GeometryCamera.fieldOfView; }
            private set { GeometryCamera.fieldOfView = ScreenshotCamera.fieldOfView = GUICamera.fieldOfView = value; }
        }

        public bool GizmoVisible
        {
            get { return Gizmo.gameObject.activeSelf; }
            set { Gizmo.gameObject.SetActive(value); }
        }

        public void ToggleFloorGrid()
        {
            FloorGrid.SetActive(!FloorGrid.activeSelf);
        }

        public CameraData CameraState
        {
            get
            {
                return new CameraData(Center.position, Pivot.rotation, Slider.localPosition.z, Slider.localRotation.eulerAngles.z, FieldOfView);
            }
            set
            {
                Center.position = value.Position;
                Pivot.rotation = value.Rotation;
                Slider.localRotation = Quaternion.Euler(0, 0, value.Roll);
                Slider.localPosition = new Vector3(0, 0, value.Distance);
                FieldOfView = value.FieldOfView;
            }
        }

        private void ResetCamera()
        {
            CameraState = new CameraData();
        }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                ResetCamera();
            }
            else if (Instance != this)
            {
                Destroy(Instance);
                Instance = this;
                ResetCamera();
            }
        }

        public void Update()
        {
            // Get modifier keys
            _shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
            _ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
            _alt = Input.GetKey(KeyCode.LeftAlt);

            // Determine if the control is active
            if (_active = !EventSystem.current.IsPointerOverGameObject())
            {
                // Get control states
                _scroll = Input.GetAxis("Mouse ScrollWheel");
                _lmb = Input.GetMouseButton(0);
                _rmb = Input.GetMouseButton(1);
                _mmb = Input.GetMouseButton(2);

                float x = Input.GetAxis("Horizontal") * MovementMultiplier * 0.2f;
                float y = Input.GetAxis("Vertical") * MovementMultiplier * 0.2f;

                // Pan or zoom the camera - prefer this over rotation
                if (_mmb)
                {
                    //Pivot.Translate(new Vector3(x, y) * -0.05f, Space.Self);
                    Center.Translate(new Vector3(x, y) * -0.05f, Pivot);
                }
                // Rotate camera
                else if (_rmb)
                {
                    if (_alt)
                    {
                        Slider.Rotate(new Vector3(0, 0, x), Space.Self);
                    }
                    else
                    {
                        Pivot.Rotate(new Vector3(0, x), Space.World);
                        Pivot.Rotate(new Vector3(-y, 0), Space.Self);
                    }
                }

                // Zoom with scroll wheel
                if (_scroll > 0)
                {
                    if (_alt)
                        FieldOfView += 2.5f;
                    else
                        Slider.Translate(new Vector3(0, 0, MovementMultiplier), Space.Self);
                }
                else if (_scroll < 0)
                {
                    if (_alt)
                        FieldOfView -= 2.5f;
                    else
                        Slider.Translate(new Vector3(0, 0, -MovementMultiplier), Space.Self);
                }
            }

            // Handle keyboard controls

            // Select bone
            if(Input.GetKeyDown(Settings.Current.Keyboard.Select))
            {
                RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));
                foreach(RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.layer == Layers.Skeleton && SceneController.Instance.SelectedModel != null && hit.collider.transform.IsChildOf(SceneController.Instance.SelectedModel.transform))
                    {
                        SceneController.Instance.SelectBone(hit.transform.GetComponent<PmxBoneBehaviour>());
                    }
                }
            }

            // Toggle bone visibility
            if(Settings.Current.Keyboard.ToggleBoneVisible.Down())
            {
                SceneController.Instance.SelectedModel.SetBonesInteractive(_bonesShown = !_bonesShown);
            }

            // Reset camera
            if (Settings.Current.Keyboard.ResetCamera.Down())
            {
                ResetCamera();
                Log.Info("Camera reset");
            }

            // Select object
            if (Settings.Current.Keyboard.SelectObject.Down())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray);
                for (int i = 0; i < hits.Length; ++i)
                {
                    SceneObject c = hits[i].transform.GetComponentInParent<SceneObject>();
                    if (c != null)
                        SceneController.Instance.SelectModel(c);
                }
            }

            // Set the gizmo's scale
            float s = -Slider.localPosition.z / 5;
            Gizmo.localScale = new Vector3(s, s, s);
        }
    }
}
