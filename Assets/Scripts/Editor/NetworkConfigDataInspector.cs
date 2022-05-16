using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

[CustomEditor(typeof(NetworkingConfigData))]
public class NetworkConfigDataInspector : Editor
{
    private NetworkingConfigData data;

    public override void OnInspectorGUI()
    {

        data = target as NetworkingConfigData;

        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Get Token"))
        {
            data.UpdateAccessTokenFromKeycloak().Forget();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        base.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();
    }


}


