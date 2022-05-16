using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Odyssey
{
    public class OddLogService : ILogService
    {

        public void Log(string msg, LogMsgType type, int level)
        {
            Debug.Log(type + " // " + msg);
        }

        public void LogError(string msg, LogMsgType type, int level)
        {
            Debug.LogError("Error in " + type + ": " + msg);
        }
    }
}
