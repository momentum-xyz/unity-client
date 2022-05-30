using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Odyssey
{

    public static class Logging
    {
        public static Action<string, LogMsgType, int> logEvent;
        public static Action<string, LogMsgType, int> logErrorEvent;

        static Logging()
        {
            // add the default logger to the Log events
            ILogService defaultLogger = LogServiceFactory.CreateDefault();

            logEvent += defaultLogger.Log;
            logErrorEvent += defaultLogger.LogError;
        }

        public static void Log(string msg, int level)
        {
            logEvent?.Invoke(msg, LogMsgType.GLOBAL, level);
        }

        public static void Log(string msg, LogMsgType type = LogMsgType.GLOBAL, int level =0)
        {
            logEvent?.Invoke(msg, type, level);
        }
      
        public static void LogError(string msg, LogMsgType type=LogMsgType.GLOBAL, int level=0)
        {
            logErrorEvent?.Invoke(msg, type, level);
        }

        public static void LogToFile(string msg, string fileName)
        {
            using (StreamWriter w = System.IO.File.AppendText(fileName))
            {
                w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}" + " " + msg);
            }
        }

    }
}
