using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Odyssey.MomentumCommon.JavaScript
{
    // [CustomEditor(typeof(JSComponent))]
    // [CanEditMultipleObjects]
    // public class JSComponentEditor : Editor
    // {
    //     //
    //     //
    //     // Static properties

    //     public static List<string> jsBehaviours { get; private set; } = new List<string>();

    //     //
    //     //
    //     // Privates

    //     private int selectedScriptI;

    //     //
    //     SerializedProperty jsBehaviour;
    //     SerializedProperty parameters;

    //     //
    //     //
    //     // Methods

    //     void OnEnable()
    //     {
    //         jsBehaviour = serializedObject.FindProperty("jsBehaviour");
    //         parameters = serializedObject.FindProperty("parameters");
    //     }

    //     public override void OnInspectorGUI()
    //     {
    //         serializedObject.Update();

    //         GUILayout.Label("Availabe ES behaviours");
    //         for (int i = 0; i < jsBehaviours.Count; ++i)
    //             if (GUILayout.Button(jsBehaviours[i])) SelectBehaviour(i);


    //         EditorGUILayout.PropertyField(jsBehaviour);
    //         EditorGUILayout.PropertyField(parameters);

    //         serializedObject.ApplyModifiedProperties();
    //     }

    //     private void SelectBehaviour(int i)
    //     {
    //         if (i < 0) i = 0;
    //         if (i > jsBehaviours.Count - 1) i = jsBehaviours.Count;

    //         selectedScriptI = i;
    //         jsBehaviour.stringValue = jsBehaviours[selectedScriptI];
    //     }
    // }
}
