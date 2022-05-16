using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCommand : DebugCommandBase
{
    private Action command;
    public DebugCommand(string commandID, string commandDescription, string commandFormat, Action command) :
        base(commandID, commandDescription, commandFormat)
    {
        this.command = command;
    }

    public void Invoke() =>
        command.Invoke();

}

public class DebugCommand<T> : DebugCommandBase
{
    private Action<T> command;
    public DebugCommand(string commandID, string commandDescription, string commandFormat, Action<T> command) :
        base(commandID, commandDescription, commandFormat)
    {
        this.command = command;
    }

    public void Invoke(T value) =>
        command.Invoke(value);

}