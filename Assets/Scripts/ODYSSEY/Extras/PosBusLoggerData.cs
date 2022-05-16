using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;

[Serializable]
public class PosBusLoggerMsg
{
    public DateTime dateTime = DateTime.Now;
    public string type;
    public string msg;
}

[CreateAssetMenu]
public class PosBusLoggerData : ScriptableObject
{
    public bool hideDetails = false;
    public bool showPositionMessage = false;
    public List<PosBusLoggerMsg> messages;

    public void SaveToFile()
    {
        string fileName = Application.dataPath + "/../../posbusMessages.log";

        StringBuilder sb = new StringBuilder();

        for (var i = 0; i < messages.Count; ++i)
        {
            sb.Append(messages[i].dateTime.ToString());
            sb.Append(" ");
            sb.Append(messages[i].type.ToString());
            sb.Append("\n");
            sb.Append(messages[i].msg);
            sb.Append("\n");
        }

        File.WriteAllText(fileName, sb.ToString());
    }
}
