using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Odyssey
{
    public interface IRendermanService
    {
        public string RendermanEndpoint { get; set; }
        public string DefaultHash { get; set; }
        public UniTask<Texture2D> DownloadTexture(string hash, RendermanTextureSize size);
    }

    /*
      * Every label coresponds to max pixel amount in image, the default one for now is s5, it could be bumped to s6, if we see low quality and pixelization
      * "s1": 4096, "s2": 9216, "s3": 25600,	"s4": 65536, "s5": 193600, "s6": 577600, "s7": 1721344,	"s8": 5062500, "s9": 14745600
     */
    public enum RendermanTextureSize { s1, s2, s3, s4, s5, s6, s7, s8, s9, original };
    public class RendermanService : IRendermanService
    {
        public static int MAX_CONCURENT_DOWNLOADS = 5;

        public string RendermanEndpoint { get { return _rendermanEndpoint; } set { _rendermanEndpoint = value; } }
        public string DefaultHash { get { return _defaultHash; } set { _defaultHash = value; } }
        private string _rendermanEndpoint;
        private string _defaultHash;

        public int downloadsRunning = 0;

        public async UniTask<Texture2D> DownloadTexture(string hash, RendermanTextureSize size)
        {
            // Dont allow to run more than 5 downloads at the same time
            // and wait until we have less running to run the next one

            await UniTask.WaitUntil(() => downloadsRunning < MAX_CONCURENT_DOWNLOADS);

            if (hash == null || hash.Length == 0) hash = _defaultHash;

            string url = this.RendermanEndpoint + "/" + TextureSizeToURIPrefix(size) + hash;

            // Debug.Log("Downloading: " + url);

            try
            {
                downloadsRunning++;
                using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
                {
                    await www.SendWebRequest();
                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        Texture2D texture = DownloadHandlerTexture.GetContent(www);
                        downloadsRunning--;
                        return texture;
                    }
                    else
                    {
                        throw new Exception(www.result.ToString());
                    }

                }
            }
            catch (Exception ex)
            {
                downloadsRunning--;
                Logging.Log("[RendermanService] Could not download texture with hash: " + hash + "." + ex.Message);
                throw new Exception("[RendermanService] Could not download texture with hash: " + hash + "." + ex.Message);
            }
        }

        private string TextureSizeToURIPrefix(RendermanTextureSize size)
        {
            switch (size)
            {
                case RendermanTextureSize.s1: return "texture/s1/";
                case RendermanTextureSize.s2: return "texture/s2/";
                case RendermanTextureSize.s3: return "texture/s3/";
                case RendermanTextureSize.s4: return "texture/s4/";
                case RendermanTextureSize.s5: return "texture/s5/";
                case RendermanTextureSize.s6: return "texture/s6/";
                case RendermanTextureSize.s7: return "texture/s7/";
                case RendermanTextureSize.s8: return "texture/s8/";
                case RendermanTextureSize.s9: return "texture/s9/";
                case RendermanTextureSize.original: return "get/";
            }

            return "";
        }
    }
}
