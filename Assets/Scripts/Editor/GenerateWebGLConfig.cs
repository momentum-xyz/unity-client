using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using Cysharp.Threading.Tasks;

public class MyBuildPostprocessor
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {

        string pathToNetworkingConfig = "Assets/Scripts/ODYSSEY/Data/Networking/WebGLLocalConfiguration.asset";

        NetworkingConfigData data = AssetDatabase.LoadAssetAtPath<NetworkingConfigData>(pathToNetworkingConfig) as NetworkingConfigData;

        if (data == null)
        {
            Debug.LogError("Could not find Networking Configuration Data..");
            return;
        }

        GetTokenAndUpdateConfigFile(data, pathToBuiltProject).Forget();


    }

    static async UniTask GetTokenAndUpdateConfigFile(NetworkingConfigData data, string pathToBuildProject)
    {
        string pathToConfig = pathToBuildProject + Path.DirectorySeparatorChar + "js" + Path.DirectorySeparatorChar + "config.js";

        await data.UpdateAccessTokenFromKeycloak();

        if (File.Exists(pathToConfig))
        {
            File.WriteAllText(pathToConfig, GenerateConfig(data));
            Debug.Log("Config saved at: " + pathToConfig);
        }
        else
        {
            Debug.LogError("config.js not found..");
        }
    }

    static string GenerateConfig(NetworkingConfigData data)
    {
        string s = "";

        s += "domain='" + data.localDomainOverwrite + "';\n";
        s += "authToken='" + data.AuthenticationToken + "';\n";
        s += "overwritePosbusURL=false;\n";
        s += "posbusURL=\"\";\n";
        s += "overwriteAddressablesURL=" + (data.overwriteWebGLAddressables ? "true" : "false") + ";\n";
        s += "addressablesURL=\"" + data.overwriteWebGLAddressablesURL + "\";\n";
        return s;
    }
}
