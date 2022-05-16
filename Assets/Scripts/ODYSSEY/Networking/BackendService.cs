using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using System.Text;

namespace Odyssey
{
    public interface IBackendService
    {
        string APIEndpoint { get; set; }
        public UniTask<WorldsMetadata> GetWorldsList();
        public UniTask<UserMetadata> GetUserData(string userID);
        public UniTask AddNewSpace(string parentGuid, bool isRoot, string name, string spaceType, string authToken);
    }

    public class BackendService : IBackendService
    {
        public string APIEndpoint { get { return _apiEndpoint; } set { _apiEndpoint = value; } }

        private string _apiEndpoint;

        public BackendService()
        {
            _userDataCache = new Dictionary<string, UserMetadata>();
        }

        public async UniTask<WorldsMetadata> GetWorldsList()
        {
            string apiURL = this._apiEndpoint + "/backend/space/worlds";
            try
            {
                string json = await GetJSONFromBackendURL(apiURL);
                if (json == null) return null;
                var worldsMetadata = JsonUtility.FromJson<WorldsMetadata>(json);
                return worldsMetadata;
            }
            catch (System.Exception ex)
            {
                throw new Exception("Could not download world data. " + ex.Message);
            }

        }

        public async UniTask<UserMetadata> GetUserData(string userID)
        {
            if (_userDataCache.ContainsKey(userID))
            {
                return _userDataCache[userID];
            }

            string apiURL = this._apiEndpoint + "/backend/users/profile/" + userID;
            try
            {
                var json = await GetJSONFromBackendURL(apiURL);
                var userMeta = JsonUtility.FromJson<UserMetadata>(json);
                _userDataCache[userID] = userMeta;
                return userMeta;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not download user data." + ex.Message);
            }
        }

        private async UniTask<string> GetJSONFromBackendURL(string url)
        {
            var result = await UnityWebRequest.Get(url).SendWebRequest();
            if (result.result != UnityWebRequest.Result.Success)
            {
                throw new Exception("Could not download data from " + url);
            }

            return result.downloadHandler.text;
        }

        public async UniTask AddNewSpace(string parentGuid, bool isRoot, string name, string spaceType, string authToken)
        {
#if UNITY_EDITOR
            string apiURL = this._apiEndpoint + "/backend/space/create";

            Debug.Log(apiURL);

            string payload = "{ ";
            payload += "\"parentId\": \"" + parentGuid + "\",";
            payload += "\"root\": false,";
            payload += "\"name\": \"" + name + "\",";
            payload += "\"spaceType\": \"" + spaceType + "\"";
            payload += " }";

            Debug.Log(payload);

            UnityWebRequest post = new UnityWebRequest();
            post.url = apiURL;
            post.method = "POST";
            post.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(payload));
            post.downloadHandler = new DownloadHandlerBuffer();

            post.SetRequestHeader("Authorization", "Bearer " + authToken);
            post.SetRequestHeader("Content-type", "application/json");

            await post.SendWebRequest();

            Debug.Log(post.result);
#else
            throw new Exception("This function is Editor only!");
#endif

        }


        Dictionary<string, UserMetadata> _userDataCache;
    }


    [Serializable]
    public class WorldsMetadata
    {
        public string[] data;
    }


    [Serializable]
    public struct UserMetadata
    {
        public string id;
        public string name;
        public string created_at;
        public string updated_at;
        public string ct_id;
        public string firstName;
        public string lastName;
        public string city;
        public string country;
        public string gender;
        public string graphicsSettings;
        public string phone;
        public string email;
        public string organisation;
        public string job_description;
        public string bio;
        public string fav_food;
        public string fav_beverage;
        public string type;
        public string role;
        public string portrait;
        public string team;
    }
}
