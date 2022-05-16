using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCommandBase
{
    private string commandID;
    private string commandDescription;
    private string commandFormat;

    public string CommandID { get { return commandID; } }
    public string CommandDescription { get { return commandDescription; } }
    public string CommandFormat { get { return commandFormat; } }

    public DebugCommandBase(string commandID, string commandDescription, string commandFormat)
    {
        this.commandID = commandID;
        this.commandDescription = commandDescription;
        this.commandFormat = commandFormat;
    }
}
