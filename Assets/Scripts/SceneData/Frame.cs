using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace MUPS.Scene
{
    /// <summary>
    /// A frame defines the state of the scene.
    /// </summary>
    [Serializable]
    public class Frame
    {
        public CameraData Camera { get; set; }
        public List<ModelPose> Models { get; set; }

        // Default frame data
        public Frame()
        {
            Camera = new CameraData();
            Models = new List<ModelPose>();
        }

        /// <summary>
        /// Applies the frame data to the scene.
        /// </summary>
        public void Apply()
        {
            ViewController.Instance.CameraState = Camera;
            foreach (ModelPose mp in Models)
            {
                mp.Apply();
            }
        }

        public static Frame GetCurrent()
        {
            Frame f = new Frame();

            f.Camera = ViewController.Instance.CameraState;
            foreach(SceneObject model in SceneController.Instance.SceneModels)
            {
                f.Models.Add(new ModelPose(model));
            }

            return f;
        }
    }
}
