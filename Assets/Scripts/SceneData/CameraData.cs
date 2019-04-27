using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MUPS.Scene
{
    /// <summary>
    /// Defines the camera's state in a single frame.
    /// </summary>
    [Serializable]
    public class CameraData
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public float Distance { get; set; }
        public float Roll { get; set; }
        public float FieldOfView { get; set; }

        public CameraData()
        {
            Position = new Vector3(0, 1, 0);
            Rotation = new Quaternion();
            Roll = 0;
            Distance = -5;
            FieldOfView = 60;
        }

        public CameraData(Vector3 position, Quaternion rotation, float distance, float roll, float fieldOfView)
        {
            Position = position;
            Rotation = rotation;
            Distance = distance;
            Roll = roll;
            FieldOfView = fieldOfView;
        }
    }
}
