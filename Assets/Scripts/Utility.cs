using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

namespace Assets.Scripts
{
    class Utility
    {
        public enum LogLevel { Trace, Debug, Info, Warning, Error, Fatal }
        public static LogLevel MinimumLogLevel = LogLevel.Trace;

        private static StreamWriter _logger;

        public static void Log(object message, LogLevel level = LogLevel.Info)
        {
            if(level >= MinimumLogLevel)
            {
                _logger.WriteLine(message.ToString());
                Debug.Log(message);
            }
        }

        static Utility()
        {
            _logger = new StreamWriter(Path.Combine(Application.persistentDataPath, "mups.log"), true, Encoding.Unicode);
        }
    }
}
