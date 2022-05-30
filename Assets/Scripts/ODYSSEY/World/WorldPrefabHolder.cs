using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using HS;
using System.Linq;

namespace Odyssey
{
    public interface IWorldPrefabHolder
    {
        public void Init(IMomentumContext context);
        public UniTask<GameObject> GetAssetAsync(string key);
        public GameObject GetMissingAssetFallback();
        public GameObject GetMissingAvatarControllerFallback();

        public void AddForPreload(Guid assetId);
        public UniTask PreloadAssets();

        public void ClearPreloadList();

    }

    public class WorldPrefabHolder : MonoBehaviour, IWorldPrefabHolder, IRequiresContext
    {
        public const string MissingAssetPath = "Prefabs/missing_model";
        public const string MissingAvatarPath = "Prefabs/3rdPersonAvatarController";

        private List<Guid> assetToPreload = new List<Guid>();

        private GameObject missingAssetPrefab;
        private GameObject missingAvatarPrefab;

        IMomentumContext _c;

        public void Init(IMomentumContext context)
        {
            this._c = context;
        }

        public async UniTask<GameObject> GetAssetAsync(string key)
        {
            key = key.Trim();

            GameObject assetPrefab = await _c.Get<IAddressablesProvider>().Get(key);

            if (assetPrefab != null)
            {
                return assetPrefab;
            }

            Logging.Log("Asset " + key + "  not found as Addressable.");

            return GetMissingAssetFallback();
        }

        public GameObject GetMissingAssetFallback()
        {
            if (missingAssetPrefab == null)
            {
                missingAssetPrefab = Resources.Load<GameObject>(MissingAssetPath) as GameObject;
            }

            if (missingAssetPrefab != null) return missingAssetPrefab;

            return new GameObject();
        }

        public GameObject GetMissingAvatarControllerFallback()
        {
            if (missingAvatarPrefab == null)
            {
                missingAvatarPrefab = Resources.Load<GameObject>(MissingAvatarPath) as GameObject;
            }

            if (missingAvatarPrefab != null) return missingAvatarPrefab;

            return null;
        }




        public void AddForPreload(Guid assetId)
        {
            if (assetToPreload.Contains(assetId)) return;
            assetToPreload.Add(assetId);
        }

        public async UniTask PreloadAssets()
        {
            for (var i = 0; i < assetToPreload.Count; ++i)
            {
                await GetAssetAsync(assetToPreload[i].ToString());
            }
        }

        public void ClearPreloadList()
        {
            assetToPreload.Clear();
        }
    }
}

