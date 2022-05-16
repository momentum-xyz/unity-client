using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
#endif

[CreateAssetMenu]
public class NetworkingConfigData : ScriptableObject
{
    [NonSerialized]
    public string AuthenticationToken;

    [Header("Neworking")]
    public string domain;
    public string localDomainOverwrite = "";

    [Header("API URI")]
    public string apiURI = "/api/v3";

    [Header("PosBus")]
    public string posBusURL;

    [Header("APIs")]
    public string rendermanURL = "";// renderman base path
    public string userEndpoint = ""; // user end point base path
    public string apiEndpoint = "";

    [Header("Addressables")]
    public string addressablesURL = "http://localhost/addr/";

    public bool useMockData = false;
    public MockData mockData;

    public bool useMockAddressables = false;
    public MockAddressablesData mockAddressableData;

    [Header("Additional WebGL Settings")]
    public bool overwriteWebGLAddressables = false;
    public string overwriteWebGLAddressablesURL = "";
    public bool ignoreApplicationURL = false; // Use the settings defined in the datafile on a WebGL build, and not the ApplicationURL

    public void InitFromDomain(string domain)
    {
        this.domain = domain;
        this.posBusURL = "wss://" + domain + "/posbus";
        this.rendermanURL = "https://" + domain + apiURI + "/render";
        this.userEndpoint = "https://" + domain + apiURI + "/backend/users/profile";
        this.apiEndpoint = "https://" + domain + apiURI;
        this.addressablesURL = "https://" + domain + "/unity-assets";

#if UNITY_WEBGL && !UNITY_EDITOR
        if (this.overwriteWebGLAddressables)
        {
            this.addressablesURL = this.overwriteWebGLAddressablesURL;
        }
#endif
    }

#if UNITY_EDITOR
    public async UniTask UpdateAccessTokenFromKeycloak()
    {
        string pathToCredentialsFile = Application.dataPath + "/../develop_credentials";

        if (!System.IO.File.Exists(pathToCredentialsFile))
        {
            Debug.LogError("Missing credentials file..");
            return;
        }

        string credJson = System.IO.File.ReadAllText(pathToCredentialsFile);

        Debug.Log(credJson);
        JsonLocalDevAccount cred = JsonUtility.FromJson<JsonLocalDevAccount>(credJson);

        Debug.Log("Getting token for account: " + cred.username);

        await GetToken(cred);
    }

    async UniTask GetToken(JsonLocalDevAccount creds)
    {
        Debug.Log("Getting Token!");
        await UniTask.SwitchToMainThread();

        WWWForm formData = new WWWForm();
        formData.AddField("username", creds.username);
        formData.AddField("password", creds.password);
        formData.AddField("client_id", creds.keycloak_client_id);
        formData.AddField("grant_type", "password");

        UnityWebRequest req = UnityWebRequest.Post(creds.keycloak_url, formData);
        var result = await req.SendWebRequest();

        if (result.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(result.error);
            return;
        }

        JsonKeycloakResponse keycloak = JsonUtility.FromJson<JsonKeycloakResponse>(result.downloadHandler.text);
        Debug.Log(keycloak.access_token);
        AuthenticationToken = keycloak.access_token;
    }

    [Serializable]
    public class JsonLocalDevAccount
    {
        public string username;
        public string password;
        public string keycloak_url;
        public string keycloak_client_id;
    }

    [Serializable]
    public class JsonKeycloakResponse
    {
        public string access_token;
    }
#endif

}

