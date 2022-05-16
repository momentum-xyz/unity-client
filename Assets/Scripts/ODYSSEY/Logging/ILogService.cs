using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Odyssey
{
    public enum LogMsgType
    {
        GLOBAL,
        NETWORKING,
        SCENE,
        USER
    }

    public interface ILogService
    {
        void Log(string msg, LogMsgType type, int level);
        void LogError(string msg, LogMsgType type, int level);
    }
}
