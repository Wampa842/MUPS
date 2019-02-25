using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MUPS.UI
{
    public class CameraBehaviour : MonoBehaviour
    {
        // Singleton instance
        public CameraBehaviour Instance { get; private set; }

        // Objects
        public Transform Center;    // Camera center position, only used for panning
        public Transform Pivot;     // Camera orbit center, only rotated
        public Transform Slider;    // Camera distance from orbit center
        public Transform Gizmo;     // Camera center gizmo
        public Camera MainCamera;   // Main geometry camera
        public Camera GUICamera;    // Camera that renders the user interface
        public Camera RenderCamera; // Camera for saving screenshots

        // Inputs
        private float _scroll;
        private bool _lmb;
        private bool _rmb;
        private bool _mmb;
        private bool _shift;
        private bool _ctrl;
        private bool _alt;
        //private bool _active;
        private float _multiplier { get { return (_shift ? 3 : 1) * (_ctrl ? 0.33f : 1); } }

        public float FieldOfView
        {
            get { return MainCamera.fieldOfView; }
            private set { MainCamera.fieldOfView = RenderCamera.fieldOfView = GUICamera.fieldOfView = value; }
        }

        public bool GizmoVisible
        {
            get { return Gizmo.gameObject.activeSelf; }
            set { Gizmo.gameObject.SetActive(value); }
        }

        private void ResetCamera()
        {
            FieldOfView = 60;
            Center.position = new Vector3(0, 1, 0);
            Pivot.rotation = new Quaternion();
            Slider.rotation = new Quaternion();
            Slider.localPosition = new Vector3(0, 0, -5);
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
            // Determine if the control is active
            //if(_active = !EventSystem.current.IsPointerOverGameObject())
            if (true)
            {
                // Get control states
                _scroll = Input.GetAxis("Mouse ScrollWheel");
                _lmb = Input.GetMouseButton(0);
                _rmb = Input.GetMouseButton(1);
                _mmb = Input.GetMouseButton(2);
                _shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
                _ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
                _alt = Input.GetKey(KeyCode.LeftAlt);

                float x = Input.GetAxis("Horizontal") * _multiplier * 0.2f;
                float y = Input.GetAxis("Vertical") * _multiplier * 0.2f;

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
                        Slider.Translate(new Vector3(0, 0, _multiplier), Space.Self);
                }
                else if (_scroll < 0)
                {
                    if (_alt)
                        FieldOfView -= 2.5f;
                    else
                        Slider.Translate(new Vector3(0, 0, -_multiplier), Space.Self);
                }
            }

            // Set the gizmo's scale
            float s = -Slider.localPosition.z / 5;
            Gizmo.localScale = new Vector3(s, s, s);
        }
    }
}