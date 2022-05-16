using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    bool showConsole;
    bool showHelp;
    string input = "";
    int previousInputCount = 0;

    public static DebugCommand Debugging;
    public static DebugCommand<int> MAKEAVATAR;

    public static DebugCommand HELP;

    private static List<string> predictionList = new List<string>();

    public List<object> commandList;
    private Vector2 scrollHelp;

    private int suggestion = -1;

    private void Awake()
    {
        HELP = new DebugCommand("help", "Shows a list of commands", "help", () =>
        {
            showHelp = true;
        });

        commandList = new List<object>()
        {
            HELP,
            MAKEAVATAR,
        };
    }

    private void OnReturn()
    {
        if (!showConsole)
            return;

        HandleInput();
        input = string.Empty;
        previousInputCount = 0;
        predictionList.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
            OnReturn();

        if (Input.GetKeyUp(KeyCode.BackQuote))
            OnToggleDebug();

        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKey(KeyCode.Tab) && showConsole)
            TabSuggestion();

        if ((Input.GetKeyUp(KeyCode.UpArrow) && showConsole) || (Input.GetKeyUp(KeyCode.Tab) && Input.GetKeyUp(KeyCode.LeftControl) && showConsole))
            TabSuggestion(true);

        if (input != "")
            if (input.Length != previousInputCount)
            {
                predictionList.Clear();
                previousInputCount = input.Length;
                PredictPromptMessage();
            }
        if (input == "")
            predictionList.Clear();
    }

    private void PredictPromptMessage()
    {
        if (commandList.Count > 0)
            for (int i = 0; i < commandList.Count; i++)
                if (commandList[i] != null)
                {
                    DebugCommandBase command = (commandList[i] as DebugCommandBase);
                    if (command.CommandFormat.Contains(input.ToLower()))
                        predictionList.Add(command.CommandFormat);
                }
    }

    private void TabSuggestion(bool previous = false)
    {
        Debug.Log(" ");

        if (input == "")
        {
            Debug.Log("Return");
            return;
        }
        if (predictionList.Count < 1)
            return;

        Debug.Log("Hi");
        if (previous)
        {

            Debug.Log(suggestion);
            suggestion--;

            if (suggestion == -1)
                input = "";
            else
                input = predictionList[suggestion];
            return;
        }

        if (suggestion == predictionList.Count)
            input = predictionList[suggestion - 1];
        else
        {
            input = predictionList[suggestion++];
            Debug.Log(suggestion);
        }

    }

    private void OnToggleDebug()
    {
        showConsole = !showConsole;
    }

    private void OnGUI()
    {
        if (!showConsole)
            return;

        float y = 0;
        float x = 0;
        if (showHelp)
        {
            GUI.Box(new Rect(x, y, Screen.width, 100), "");

            Rect viewport = new Rect(x, y, Screen.width - 30, 20 * commandList.Count);
            scrollHelp = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), scrollHelp, viewport);

            for (int i = 0; i < commandList.Count; i++)
            {
                DebugCommandBase command = (commandList[i] as DebugCommandBase);
                string label = $"{command.CommandFormat} - {command.CommandDescription}";
                Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);

                GUI.Label(labelRect, label);
            }
            GUI.EndScrollView();

            y += 100;
        }


        GUI.Box(new Rect(x, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);

        if (predictionList.Count > 0)
        {
            y += 30;

            GUI.Box(new Rect(x, y, Screen.width, 100), "");

            for (int i = 0; i < predictionList.Count; i++)
            {
                GUI.backgroundColor = new Color(0, 0, 0, 0);
                Rect rect = new Rect(10f, (y + (10 * i + 1)), Screen.width - 20f, 20f);
                GUI.Label(rect, predictionList[i]);
            }
        }
    }

    /// <summary>
    /// Handle the input for the terminal
    /// </summary>
    private void HandleInput()
    {
        input.ToLower();
        string[] properties = input.Split(' ');

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if (input.Contains(commandBase.CommandID))
            {
                if (commandList[i] as DebugCommand != null)
                    (commandList[i] as DebugCommand).Invoke();
                else if ((commandList[i] as DebugCommand<int>) != null)
                    (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
            }
        }
    }
}
