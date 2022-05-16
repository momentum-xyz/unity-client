using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Cysharp.Threading.Tasks;

[CustomEditor(typeof(KusamaMockData))]
public class KusamaMockDataInspector : Editor
{
    private KusamaMockData _mockData;

    public string _staticEffectEmitter = "";
    private int _staticEffectAssetGUID;
    private string _staticEffectSourceGUID = "99999999-0001-1111-1111-111111111111";
    private int _staticEffectType = 0;
    private string _spaceDeleteGUID;

    private string _transitionEffectEmitter = "";
    private string _transitionEffectSource = "99999999-9999-1111-1111-111111100000";
    private string _transitionEffectDestination = "99999999-8888-1111-1111-111111100000";
    private int _transitionEffectType = 0;
    private int _transitionEffectAssetIDIdx;

    public override void OnInspectorGUI()
    {
        _mockData = (KusamaMockData)target;
        serializedObject.Update();

        base.OnInspectorGUI();

        EditorGUILayout.Space();
        GUILayout.Label("Blocks", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Block"))
        {
            _mockData.AddBlock();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Simuilate Block"))
        {
            _mockData.SimulateBlock().Forget();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        GUILayout.Label("Users", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Random User"))
        {
            _mockData.AddUser();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        GUILayout.Label("World Objects", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        _spaceDeleteGUID = EditorGUILayout.TextField("GUID:", _spaceDeleteGUID);
        if (GUILayout.Button("Delete Object"))
        {
            _mockData.DeleteObject(_spaceDeleteGUID);
        }

        if (Application.isPlaying)
        {
            EditorGUILayout.Space();
            GUILayout.Label("Effects", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _staticEffectEmitter = EditorGUILayout.TextField("Emitter ID: ", _staticEffectEmitter);
            _staticEffectSourceGUID = EditorGUILayout.TextField("Position Obj GUID:", _staticEffectSourceGUID);
            _staticEffectType = EditorGUILayout.IntField("Type: ", _staticEffectType);

            string[] _effectAssetsIDs = new string[_mockData.effectsAssets.Length];

            for (var i = 0; i < _mockData.effectsAssets.Length; ++i)
            {
                _effectAssetsIDs[i] = _mockData.effectsAssets[i].name;
            }

            _staticEffectAssetGUID = EditorGUILayout.Popup("Effect Asset", _staticEffectAssetGUID, _effectAssetsIDs);


            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Static Effect"))
            {
                _mockData.StaticEffect(Guid.Parse(_staticEffectEmitter), Guid.Parse(_staticEffectSourceGUID), Guid.Parse(_mockData.effectsAssets[_staticEffectAssetGUID].id), _staticEffectType);
            }

            EditorGUILayout.EndHorizontal();

            _transitionEffectEmitter = EditorGUILayout.TextField("Emitter ID: ", _transitionEffectEmitter);
            _transitionEffectSource = EditorGUILayout.TextField("Source GUID:", _transitionEffectSource);
            _transitionEffectDestination = EditorGUILayout.TextField("Destination GUID:", _transitionEffectDestination);
            _transitionEffectType = EditorGUILayout.IntField("Effect Type:", _transitionEffectType);

            string[] __effectAssetsIDs = new string[_mockData.effectsAssets.Length];

            for (var i = 0; i < _mockData.effectsAssets.Length; ++i)
            {
                __effectAssetsIDs[i] = _mockData.effectsAssets[i].name;
            }

            _transitionEffectAssetIDIdx = EditorGUILayout.Popup("Effect Asset", _transitionEffectAssetIDIdx, __effectAssetsIDs);


            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Transition Effect"))
            {
                _mockData.TransitionEffect(Guid.Parse(_transitionEffectEmitter), Guid.Parse(_transitionEffectSource), Guid.Parse(_transitionEffectDestination), Guid.Parse(_mockData.effectsAssets[_transitionEffectAssetIDIdx].id), _transitionEffectType);
            }

            EditorGUILayout.EndHorizontal();


        }


        serializedObject.ApplyModifiedProperties();
    }
}
