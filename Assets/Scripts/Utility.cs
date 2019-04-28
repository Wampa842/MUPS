using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using MUPS.SaveData;

namespace MUPS
{
    public static class MUPSExtensions
    {
        /// <summary>
        /// Reorients the Transform to the specified rotation without affecting its children.
        /// </summary>
        public static void Reorient(this Transform t, Quaternion rot, bool local = false)
        {
            // Detach children
            List<Transform> list = new List<Transform>();
            foreach (Transform child in t)
            {
                list.Add(child);
            }
            t.DetachChildren();

            // Reorient
            if (local)
            {
                t.localRotation = rot;
            }
            else
            {
                t.rotation = rot;
            }

            // Reattach children
            foreach (Transform child in list)
            {
                child.SetParent(t);
            }
        }
    }

    public static class Layers
    {
        public static int Skeleton { get { return 17; } }
        public static int Gizmos { get { return 12; } }
        public static int BoneSprites { get { return 18; } }
    }

    public static class Log
    {
        public static string LogPath { get; } = Path.Combine(Application.persistentDataPath, "mups.log");
        public static bool LogWriteFailed { get; private set; } = false;

        public enum LogLevel { Trace = 1, Debug = 2, Info = 3, Warning = 4, Error = 5, Fatal = 6, Disabled = 7 }
        public static LogLevel MinimumLevel { get; set; } = LogLevel.Trace;

        public static void WriteLog(string line)
        {
            if (LogWriteFailed)
                return;

            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(LogPath, true, Encoding.Unicode);
                writer.WriteLine(line);
            }
            catch(Exception ex)
            {
                UnityEngine.Debug.LogErrorFormat("Log file unavailable: {0}", ex.ToString());
                LogWriteFailed = true;
            }
            finally
            {
                writer.Close();
            }

        }

        public static void Trace(params object[] message)
        {
            if (MinimumLevel > LogLevel.Trace)
                return;

            string line = string.Join("; ", message);
            WriteLog(line);
            UnityEngine.Debug.Log(line);
        }

        public static void Debug(params object[] message)
        {
            if (MinimumLevel > LogLevel.Debug)
                return;

            string line = string.Join("; ", message);
            WriteLog(line);
            UnityEngine.Debug.Log(line);
        }

        public static void Info(params object[] message)
        {
            if (MinimumLevel > LogLevel.Info)
                return;

            string line = string.Join("; ", message);
            WriteLog(line);
            UnityEngine.Debug.Log(line);
        }

        public static void Warning(params object[] message)
        {
            if (MinimumLevel > LogLevel.Warning)
                return;

            string line = string.Join("; ", message);
            WriteLog(line);
            UnityEngine.Debug.LogWarning(line);
        }

        public static void Error(params object[] message)
        {
            if (MinimumLevel > LogLevel.Error)
                return;

            string line = string.Join("; ", message);
            WriteLog(line);
            UnityEngine.Debug.LogError(line);
        }

        public static void Fatal(params object[] message)
        {
            if (MinimumLevel > LogLevel.Fatal)
                return;

            string line = string.Join("; ", message);
            WriteLog(line);
            UnityEngine.Debug.LogError(line);
        }
    }
}
