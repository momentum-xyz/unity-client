using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;

public class GenerateEnum : MonoBehaviour
{
    [Tooltip("Whats the generated Classname")]
    [SerializeField]
    private string className;

    [ContextMenu("Generate")]
    private void Generate()
    {
        //Change these strings to generate the enum in another folder or directory
        string directoryPath = Environment.CurrentDirectory + "/Assets/___MOMENTUM3D___/Scripts/PrefabEnum.cs";
        string prefabFolder = Environment.CurrentDirectory + "/Assets/___MOMENTUM3D___/Prefabs";
        string[] fileNames = Directory.GetFiles(prefabFolder + "/*.prefab");

        for (int i = 0; i < fileNames.Length; i++)
        {
            string[] fileName = fileNames[i].Split('/');
            string[] filenameWithoutExtension = fileName[fileName.Length - 1].Split('.');
            fileNames[i] = filenameWithoutExtension[0];
            Debug.Log(fileNames[i]);
        }

        using (StreamWriter sw = new StreamWriter(directoryPath))
        {
            sw.WriteLine($"public class { className }");
            sw.WriteLine("{");
            sw.WriteLine("\tpublic enum PoolableObjects");
            sw.WriteLine("\t{");

            for (int i = 0; i < fileNames.Length; i++)
            {
                if (fileNames.Length - 1 != i)
                    sw.WriteLine("\t\t" + fileNames[i].ToString() + ",");
                else
                    sw.WriteLine("\t\t" + fileNames[i].ToString());
            }

            sw.WriteLine("\t}");

            sw.WriteLine("}");
        }
    }
}
