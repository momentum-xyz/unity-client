using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Odyssey;
using Odyssey.Networking;
using System;
using System.Reflection;

public class PosBusLoggerWindow : EditorWindow
{
    static IMomentumContext context;
    static bool subscribedToPosBus = false;


    static PosBusLoggerData data;

    private Vector2 scrollPos;

    GUIStyle msgTypeStyle;

    string filterQuery = "";
    string filterType = "";


    public static void InitContext(IMomentumContext ctx)
    {
        context = ctx;
    }

    [MenuItem("ODYSSEY/PosBusLogger")]
    static void Init()
    {
        var window = (PosBusLoggerWindow)EditorWindow.GetWindow(typeof(PosBusLoggerWindow));
        window.Show();
    }

    private void Awake()
    {
        CreateStyles();

        subscribedToPosBus = false;

        GUIStyle TextFieldStyles = new GUIStyle(EditorStyles.textField);
    }

    void CreateStyles()
    {
        msgTypeStyle = new GUIStyle(EditorStyles.textField);
        msgTypeStyle.normal.textColor = Color.white;
        msgTypeStyle.normal.background = MakeTex(32, 32, Color.black);

    }

    void LoadData()
    {

        data = AssetDatabase.LoadAssetAtPath<PosBusLoggerData>("Assets/Scripts/Editor/PosBusLoggerData.asset");

    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }



    void OnGUI()
    {
        if (data == null) LoadData();

        if (data == null)
        {
            Debug.Log("PosbusLoggerData is missing...");
            return;
        }

        if (msgTypeStyle == null)
        {
            CreateStyles();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear"))
        {
            data.messages.Clear();
            EditorUtility.SetDirty(data);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save To File"))
        {
            data.SaveToFile();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        data.showPositionMessage = EditorGUILayout.Toggle("Show Position Messages", data.showPositionMessage);
        data.hideDetails = EditorGUILayout.Toggle("Hide Details", data.hideDetails);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        filterType = EditorGUILayout.TextField("Filter Type: ", filterType);
        filterQuery = EditorGUILayout.TextField("Filter Data: ", filterQuery);

        EditorGUILayout.BeginHorizontal();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height - 100.0f));

        for (var i = 0; i < data.messages.Count; ++i)
        {
            if (filterQuery.Length > 0)
            {
                if (!data.messages[i].msg.Contains(filterQuery)) continue;
            }

            if (filterType.Length > 0)
            {
                if (!data.messages[i].type.Contains(filterType))
                {
                    continue;
                }
            }
            DrawLogMsg(data.messages[i], !data.hideDetails);

        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();

        this.Repaint();

    }

    void DrawLogMsg(PosBusLoggerMsg m, bool showDetails)
    {

        EditorGUILayout.LabelField(m.dateTime.ToString("T") + ": " + m.type, msgTypeStyle);
        if (showDetails) EditorGUILayout.TextArea(m.msg);
    }

    static void OnPosBusMessage(IPosBusMessage msg)
    {

        if (data != null)
        {
            if ((msg.GetType() == typeof(PosBusPosMsg)) && !data.showPositionMessage) return;

            data.messages.Add(new PosBusLoggerMsg()
            {
                type = msg.GetType().ToString(),
                msg = GetAllFields(msg.GetType(), msg)
            }); ;
        }

        EditorUtility.SetDirty(data);

    }

    static string GetAllFields(Type t, System.Object o)
    {
        string s = "";

        if (t.IsPrimitive)
        {
            return o.ToString() + "\n";
        }

        if (t == typeof(Guid))
        {
            return o.ToString() + "\n";

        }

        FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo f in fields)
        {
            s += f.Name + " " + f.GetValue(o).ToString() + " \n";

            int idx = 0;
            if (f.GetValue(o).GetType().IsArray)
            {
                Array list = (Array)f.GetValue(o);
                foreach (var l in list)
                {
                    s += "element: " + idx + "\n";
                    s += GetAllFields(l.GetType(), l);
                    s += "-------\n";
                    idx++;
                }

            }
        }

        return s;
    }

    private void Update()
    {
        if (Application.isPlaying)
        {

            if (PosBusLogger.PosBus != null && !subscribedToPosBus)
            {
                PosBusLogger.PosBus.OnPosBusMessage += OnPosBusMessage;
                subscribedToPosBus = true;
                data.messages.Clear();
                EditorUtility.SetDirty(data);
            }

            Repaint();
        }
        else
        {
            if (subscribedToPosBus)
            {
                subscribedToPosBus = false;
            }
        }


    }

    private void OnDestroy()
    {

        if (Application.isPlaying && PosBusLogger.PosBus != null && subscribedToPosBus)
        {
            PosBusLogger.PosBus.OnPosBusMessage -= OnPosBusMessage;
        }

        data = null;
    }
}


