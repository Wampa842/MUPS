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
    public static class Utility
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

        public static bool ParseHtmlColor(string s, out Color color)
        {
            string line = s.Trim().TrimStart('#');
            switch (line.Length)
            {
                case 3:
                case 4:
                case 6:
                case 8:
                    break;
                default:
                    color = Color.black;
                    return false;
            }

            if (line.Length == 6)
            {
                int r = Convert.ToInt32(line.Substring(0, 2), 16);
                int g = Convert.ToInt32(line.Substring(2, 2), 16);
                int b = Convert.ToInt32(line.Substring(4, 2), 16);
                color = new Color(r / 255.0f, g / 255.0f, b / 255.0f, 1);
            }
            else if (line.Length == 8)
            {
                int r = Convert.ToInt32(line.Substring(0, 2), 16);
                int g = Convert.ToInt32(line.Substring(2, 2), 16);
                int b = Convert.ToInt32(line.Substring(4, 2), 16);
                int a = Convert.ToInt32(line.Substring(6, 2), 16);
                color = new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
            }
            else if (line.Length == 3)
            {
                int r = Convert.ToInt32(string.Concat(line[0], line[0]), 16);
                int g = Convert.ToInt32(string.Concat(line[1], line[1]), 16);
                int b = Convert.ToInt32(string.Concat(line[2], line[2]), 16);
                color = new Color(r / 255.0f, g / 255.0f, b / 255.0f, 1);
            }
            else
            {
                int r = Convert.ToInt32(string.Concat(line[0], line[0]), 16);
                int g = Convert.ToInt32(string.Concat(line[1], line[1]), 16);
                int b = Convert.ToInt32(string.Concat(line[2], line[2]), 16);
                int a = Convert.ToInt32(string.Concat(line[3], line[3]), 16);
                color = new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
            }

            return true;
        }

        public static string ToHtml(this Color color)
        {
            byte r = (byte)(color.r * 255.0f);
            byte g = (byte)(color.g * 255.0f);
            byte b = (byte)(color.b * 255.0f);
            byte a = (byte)(color.a * 255.0f);

            return r.ToString("X2") + g.ToString("X2") + b.ToString("X2") + a.ToString("X2");
        }

        public static Color RandomColor()
        {
            return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);
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
        public static System.Diagnostics.Process LogConsole { get; set; }
        public static string LogPath { get; } = Path.Combine(Application.persistentDataPath, "mups.log");
        public static bool LogWriteFailed { get; private set; } = false;

        public enum LogLevel { Trace = 1, Debug = 2, Info = 3, Warning = 4, Error = 5, Fatal = 6, Disabled = 7 }
        public static LogLevel MinimumLevel { get; set; } = LogLevel.Trace;

        public static void WriteLog(string line)
        {
            if(LogConsole != null)
            {
                LogConsole.StandardInput.WriteLine(line);
            }

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

        public static void Write(LogLevel level, params object[] message)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    Trace(message);
                    break;
                case LogLevel.Debug:
                    Debug(message);
                    break;
                case LogLevel.Info:
                    Info(message);
                    break;
                case LogLevel.Warning:
                    Warning(message);
                    break;
                case LogLevel.Error:
                    Error(message);
                    break;
                case LogLevel.Fatal:
                    Error(message);
                    break;
                default:
                    break;
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
