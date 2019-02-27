using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MUPS.UI
{
    [System.Serializable]
    public class KeyboardControls
    {
        // View controls
        public KeyCode SlowerModifier { get; private set; }
        public KeyCode FasterModifier { get; private set; }
        public KeyCode AltModifier { get; private set; }
        public KeyCode Select { get; private set; }
        public KeyCode RotateCamera { get; private set; }
        public KeyCode PanCamera { get; private set; }

        // Manipulation controls
        public KeyCode ToggleLocal { get; set; }
        public KeyCode ResetCamera { get; set; }
        public KeyCode LoadCameraState { get; set; }
        public KeyCode RegisterState { get; set; }

        public KeyboardControls()
        {
            SlowerModifier = KeyCode.LeftControl;
            FasterModifier = KeyCode.LeftShift;
            AltModifier = KeyCode.LeftAlt;
            Select = KeyCode.Mouse0;
            RotateCamera = KeyCode.Mouse1;
            PanCamera = KeyCode.Mouse2;

            ToggleLocal = KeyCode.G;
            ResetCamera = KeyCode.R;
            LoadCameraState = KeyCode.F;
            RegisterState = KeyCode.Return;
        }

        // Global access
        public static KeyboardControls Current { get; set; }
        static KeyboardControls()
        {
            Current = new KeyboardControls();
        }
    }

    /// <summary>
    /// Represents a state of the main camera.
    /// </summary>
    [System.Serializable]
    public class CameraData
    {
        public Vector3 Position { get; set; }
        public Vector4 Rotation { get; set; }
        public float Roll { get; set; }
        public float Distance { get; set; }
        public float FieldOfView { get; set; }

        public CameraData(Vector3 position, Quaternion rotation, float roll, float distance, float fieldOfView)
        {
            Position = position;
            Rotation = new Vector4(rotation.x, rotation.y, rotation.z, rotation.w);
            Roll = roll;
            Distance = distance;
            FieldOfView = fieldOfView;
        }

        public static CameraData Default { get; private set; }
        public static CameraData Stored { get; set; }
        static CameraData()
        {
            Default = new CameraData(new Vector3(0, 1, 0), new Quaternion(), 0, -5, 60);
        }
    }

    public class ViewController : MonoBehaviour
    {
        // Singleton instance
        public static ViewController Instance { get; private set; }

        // Objects
        public Transform Center;            // Camera center position, only used for panning
        public Transform Pivot;             // Camera orbit center, only rotated
        public Transform Slider;            // Camera distance from orbit center
        public Transform Gizmo;             // Camera center gizmo
        public Camera GeometryCamera;       // Main geometry camera
        public Camera GUICamera;            // Camera that renders the user interface
        public Camera ScreenshotCamera;     // Camera for saving screenshots

        // Inputs
        private bool _active;               // Control is active
        private float _scroll;              // Scroll wheel delta
        private bool _lmb;                  // Mouse 0
        private bool _rmb;                  // Mouse 1
        private bool _mmb;                  // Mouse 2
        private bool _shift;                // Left/right shift
        private bool _ctrl;                 // Left/right control
        private bool _alt;                  // Left alt

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

        private CameraData CameraState
        {
            get
            {
                return new CameraData(Center.position, Pivot.rotation, Slider.localEulerAngles.z, Slider.localPosition.z, FieldOfView);
            }
            set
            {
                Center.position = value.Position;
                Pivot.rotation = new Quaternion(value.Rotation.x, value.Rotation.y, value.Rotation.z, value.Rotation.w);
                Slider.localRotation = Quaternion.Euler(0, 0, value.Roll);
                Slider.localPosition = new Vector3(0, 0, value.Distance);
                FieldOfView = value.FieldOfView;
            }
        }

        private void ResetCamera()
        {
            CameraState = CameraData.Default;
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
            if (Input.GetKeyDown(KeyboardControls.Current.ResetCamera))
            {
                ResetCamera();
                Debug.Log("Camera reset");
            }

            if (Input.GetKeyDown(KeyboardControls.Current.RegisterState))
            {
                CameraData.Stored = CameraState;
                Debug.Log("Camera state stored");
            }

            if (Input.GetKeyDown(KeyboardControls.Current.LoadCameraState))
            {
                Debug.Log("Camera state loaded");
                CameraState = CameraData.Stored;
            }

            if (Input.GetKeyDown(KeyboardControls.Current.ToggleLocal))
            {
                Scene.SceneController.Instance.CycleReferenceSystem();
            }

            if (Input.GetKeyDown(KeyboardControls.Current.Select) && _ctrl)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray);
                for (int i = 0; i < hits.Length; ++i)
                {
                    PmxModel c = hits[i].transform.GetComponentInParent<PmxModel>();
                    if (c != null)
                        Scene.SceneController.Instance.SelectModel(c);
                }
            }

            // Set the gizmo's scale
            float s = -Slider.localPosition.z / 5;
            Gizmo.localScale = new Vector3(s, s, s);
        }
    }
}