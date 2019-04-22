using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

using UnityEngine;

namespace MUPS.SaveData
{
    [Serializable]
    public struct BonePose
    {
        public Vector3 Position;
        public Vector4 Rotation;

        public BonePose(Transform t)
        {
            Position = t.localPosition;
            Quaternion rot = t.localRotation;
            Rotation = new Vector4(rot.x, rot.y, rot.z, rot.w);
        }

        public void Apply(Transform t)
        {
            t.localPosition = Position;
            t.localRotation = new Quaternion(Rotation.x, Rotation.y, Rotation.z, Rotation.w);
        }
    }

    [Serializable]
    public class Pose
    {
        public string FileName { get; set; }
        public Dictionary<string, BonePose> Skeleton { get; set; }
        public Dictionary<string, float> Morph { get; set; }

        public Pose(PmxModelBehaviour model)
        {
            FileName = model.FileName;

            Skeleton = new Dictionary<string, BonePose>();
            foreach(PmxBoneBehaviour bone in model.GetComponentsInChildren<PmxBoneBehaviour>())
            {
                Skeleton.Add(bone.Name, new BonePose(bone.transform));
            }

            Morph = new Dictionary<string, float>();
        }

        public void Apply(PmxModelBehaviour model)
        {
            foreach(PmxBoneBehaviour bone in model.GetComponentsInChildren<PmxBoneBehaviour>())
            {
                Skeleton[bone.Name].Apply(bone.transform);
            }
        }
    }

    [Serializable]
    public class CameraData
    {
        public Vector3 Position { get; set; }
        public Vector4 Rotation { get; set; }
        public float Roll { get; set; }
        public float Distance { get; set; }
        public float FieldOfView { get; set; }

        public CameraData()
        {
            Position = new Vector3(0, 1, 0);
            Rotation = new Vector4();
            Roll = 0;
            Distance = -5;
            FieldOfView = 60;
        }

        public CameraData(Vector3 position, Quaternion rotation, float roll, float distance, float fieldOfView)
        {
            Position = position;
            Rotation = new Vector4(rotation.x, rotation.y, rotation.z, rotation.w);
            Roll = roll;
            Distance = distance;
            FieldOfView = fieldOfView;
        }
    }

    [Serializable]
    public class SceneData
    {
        public CameraData Camera { get; set; }
        public List<Pose> Poses { get; set; }

        private static SceneData _stored;
        public static SceneData Stored
        {
            get
            {
                return _stored;
            }
            private set
            {
                _stored = value;
                ViewController.Instance.SetCamera(_stored.Camera);
            }
        }

        public static void Export(string path)
        {
            StreamWriter stream = null;
            JsonWriter writer = null;
            JsonSerializer serializer = new JsonSerializer { NullValueHandling = NullValueHandling.Include };

            try
            {
                stream = new StreamWriter(path);
                writer = new JsonTextWriter(stream);
                serializer.Serialize(writer, Stored);
                Logger.Log("Scene data saved.");
            }
            catch (Exception ex)
            {
                Logger.Log("An exception occured while trying to save the scene data.", Logger.LogLevel.Error);
                Logger.Log(ex, Logger.LogLevel.Error, true);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (writer != null)
                    writer.Close();
            }
        }

        public static void Import(string path)
        {
            StreamReader stream = null;
            JsonReader reader = null;
            JsonSerializer serializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };

            try
            {
                stream = new StreamReader(path);
                reader = new JsonTextReader(stream);
                Stored = serializer.Deserialize<SceneData>(reader);
                Logger.Log("Scene loaded.");
            }
            catch (Exception ex)
            {
                Logger.Log("An exception occured while trying to load the scene data.", Logger.LogLevel.Error);
                Logger.Log(ex, Logger.LogLevel.Error, true);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (reader != null)
                    reader.Close();
            }
        }

        static SceneData()
        {
            _stored = new SceneData();
        }
    }
}
