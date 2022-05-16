using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Odyssey
{
    public static class DataHelpers
    {

        /// <summary>
        /// Strip the domain name from an URL
        /// </summary>
        /// <returns></returns>
        public static string GetDomainFromURL(string URL)
        {
            UriBuilder uriBuilder = new UriBuilder(URL);
            return uriBuilder.Host;
        }

        public static UserTokenContent DecodeToken(string token)
        {
            var parts = token.Split('.');
            if (parts.Length > 2)
            {
                var decode = parts[1];
                var padLength = 4 - decode.Length % 4;

                if (padLength < 4)
                {
                    decode += new string('=', padLength);
                }
                var bytes = System.Convert.FromBase64String(decode);
                var userInfo = System.Text.ASCIIEncoding.ASCII.GetString(bytes);

                return JsonUtility.FromJson<UserTokenContent>(userInfo);
            }
            return null;
        }

        /// <summary>
        /// Creates and returns a clone of any given scriptable object.
        /// </summary>
        public static T Clone<T>(this T scriptableObject) where T : ScriptableObject
        {
            if (scriptableObject == null)
            {
                Debug.LogError($"ScriptableObject was null. Returning default {typeof(T)} object.");
                return (T)ScriptableObject.CreateInstance(typeof(T));
            }

            T instance = UnityEngine.Object.Instantiate(scriptableObject);
            instance.name = scriptableObject.name; // remove (Clone) from name
            return instance;
        }
    }



    // used to hold jwt data
    [Serializable]
    public class UserTokenContent
    {
        public string sub;
        public string name;
    }

}
