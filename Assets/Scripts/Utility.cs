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
    public static class Layers
    {
        public static int Skeleton { get { return 17; } }
    }

    public class Logger
    {
        public enum LogLevel { Trace = 1, Debug = 2, Info = 3, Warning = 4, Error = 5, Fatal = 6, Disabled }
        public static bool LogTimestamps = true;
        public static string LogPath = Path.Combine(Application.persistentDataPath, "mups.log");
        private static bool _logFailed = false;

        public static void Log(object message, LogLevel level = LogLevel.Info, bool noTimestamp = false)
        {
            if (_logFailed)
                return;
            if (level >= Settings.Current.ApplicationSettings.MinimumLogLevel)
            {
                StreamWriter writer = new StreamWriter(LogPath, true, Encoding.Unicode);
                string ts = (LogTimestamps || noTimestamp) ? string.Format(CultureInfo.InvariantCulture.NumberFormat, "{0:0.0000} ", Time.time) : "";
                try
                {
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}{1}", ts, message));
                    writer.Close();
                }
                catch (Exception ex)
                {
                    Debug.LogWarningFormat("Logging unavailable:\n{0}", ex);
                    _logFailed = true;
                }

                switch (level)
                {
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                    case LogLevel.Info:
                        Debug.Log(message);
                        break;
                    case LogLevel.Warning:
                        Debug.LogWarning(message);
                        break;
                    case LogLevel.Error:
                        Debug.LogError(message);
                        break;
                    case LogLevel.Fatal:
                        Debug.LogError(message);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
