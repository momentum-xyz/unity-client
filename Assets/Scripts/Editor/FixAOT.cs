using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class FixAOT : IPreprocessBuildWithReport
{
    public int callbackOrder => 1;

    public void OnPreprocessBuild(BuildReport report)
    {
        // Needed in Unity 2021 to handle Generics
        PlayerSettings.SetAdditionalIl2CppArgs("--generic-virtual-method-iterations=2");
    }
}
