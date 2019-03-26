using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MUPS.Scene;

namespace MUPS.UI
{
    class TransformControlBehaviour : MonoBehaviour
    {
        public void Transform(int mode)
        {
            float delta = Input.GetAxis("Vertical") * ViewController.Instance.MovementMultiplier * 0.5f;
            Space space = SceneController.Instance.Local ? Space.Self : Space.World;

            if (SceneController.Instance.SelectedBone != null)
            {
                // Move selected object
                switch (mode)
                {
                    case 0:
                        SceneController.Instance.SelectedBone.transform.Translate(delta * 0.05f, 0, 0, space);
                        break;
                    case 1:
                        SceneController.Instance.SelectedBone.transform.Translate(0, delta * 0.05f, 0, space);
                        break;
                    case 2:
                        SceneController.Instance.SelectedBone.transform.Translate(0, 0, delta * 0.05f, space);
                        break;
                    case 3:
                        SceneController.Instance.SelectedBone.transform.Rotate(delta, 0, 0, space);
                        break;
                    case 4:
                        SceneController.Instance.SelectedBone.transform.Rotate(0, delta, 0, space);
                        break;
                    case 5:
                        SceneController.Instance.SelectedBone.transform.Rotate(0, 0, delta, space);
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
                        ViewController.Instance.Center.Translate(delta * 0.05f, 0, 0, SceneController.Instance.Local ? ViewController.Instance.Pivot : ViewController.Instance.Center);
                        break;
                    case 1:
                        ViewController.Instance.Center.Translate(0, delta * 0.05f, 0, SceneController.Instance.Local ? ViewController.Instance.Pivot : ViewController.Instance.Center);
                        break;
                    case 2:
                        ViewController.Instance.Center.Translate(0, 0, delta * 0.05f, SceneController.Instance.Local ? ViewController.Instance.Pivot : ViewController.Instance.Center);
                        break;
                    case 3:
                        ViewController.Instance.Pivot.Rotate(delta, 0, 0, space);
                        break;
                    case 4:
                        ViewController.Instance.Pivot.Rotate(0, delta, 0, Space.World);
                        break;
                    case 5:
                        if (SceneController.Instance.Local)
                            ViewController.Instance.Slider.Rotate(0, 0, delta, Space.Self);
                        else
                            ViewController.Instance.Pivot.Rotate(0, 0, delta, Space.World);
                        break;
                }
            }
        }
    }
}
