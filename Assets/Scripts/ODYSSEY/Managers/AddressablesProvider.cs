// Uncomment if you want to use the remote Addressables catalogs
#define TEST_ADDRESSABLES 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Odyssey;
using System;

namespace Odyssey
{

    public interface IAddressablesProvider
    {
        public void Init(IMomentumContext context);
        public void Add(string key, string remoteURI, string address);
        public UniTask<GameObject> Get(string key);
        public bool AssetExists(string key);
        public void Clear();

        public Dictionary<string, AddressableAsset> AddressablesAssets { get; set; }

    }
    public class AddressablesProvider : IAddressablesProvider, IRequiresContext
    {
        public const string AddressablesDBAssetPath = "Data/Assets";
        public Dictionary<string, AddressableAsset> AddressablesAssets { get { return _addressablesAssets; } set { } }
        public GameObject UnknownObject { get => new GameObject("Unknown object"); }

        IMomentumContext _c;
        Dictionary<string, AddressableAsset> _addressablesAssets = new Dictionary<string, AddressableAsset>();

        AddressablesAssetsContainer _addressablesDB = null;

        public void Init(IMomentumContext context)
        {
            this._c = context;
        }


        /// <summary>
        /// Gets the Address of the Addressable from our local DB
        /// TODO: Get this Address dynamically from the backend service
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetAddressFromDB(string key)
        {
            key = key.Trim();

            if (_addressablesDB == null) _addressablesDB = Resources.Load<AddressablesAssetsContainer>(AddressablesDBAssetPath);

            if (_addressablesDB == null)
            {
                Debug.LogError("Assets Data file not available...");
                return null;
            }

            foreach (KeyValuePair<string, List<AddressablesAsset>> kv in _addressablesDB.assets)
            {
                for (var i = 0; i < kv.Value.Count; ++i)
                {
                    if (key.Equals(kv.Value[i].guid))
                    {
                        return kv.Value[i].address;
                    }
                }
            }

            for (var i = 0; i < _addressablesDB.sharedAssets.Count; ++i)
            {
                if (key.Equals(_addressablesDB.sharedAssets[i].guid)) return _addressablesDB.sharedAssets[i].address;
            }

            return null;
        }


        /// <summary>
        /// Registers an element with a specified key.
        /// Does not load it.
        /// </summary>
        /// <param name="key">Unique reference to the game object in Content</param>
        /// <param name="remoteURI">URI to the element</param>
        public void Add(string key, string remoteURI, string address)
        {
            if (_addressablesAssets.ContainsKey(key))
            {
                Logging.Log("[ContentProvider] Content already contains element with key '" + key + "'");
                return;
            }

            _addressablesAssets.Add(key, new AddressableAsset(remoteURI, address, _c.Get<ISessionData>().NetworkingConfig.addressablesURL));

        }

        /// <summary>
        /// Returns the GameObject registered with the specified key.
        /// Ensures that the object is loaded.
        /// </summary>
        /// <param name="key">Unique reference to the game object in Content</param>
        /// <returns>game object</returns>
        public async UniTask<GameObject> Get(string key)
        {
            if(!_addressablesAssets.ContainsKey(key))
            {
                string address = GetAddressFromDB(key);

                if(address == null)
                {
                    Logging.Log("[AddressablesProvider] Could not find addressable asset for:" + key);
                    return null;
                }

                Add(key, _c.Get<ISessionData>().NetworkingConfig.addressablesURL + "/" + GetPlatformPrefix() + "/catalog_" + address + ".json", address);

            }

            AddressableAsset asset = _addressablesAssets[key];

            // Load the object if it is not loaded..
            if (
                asset.status == AddressableAssetStatus.NotLoaded ||
                asset.status == AddressableAssetStatus.Failed
            )
            {
                await asset.Load();
            }

            // await until the object is loaded, if somebody already requested for it
            if (asset.status == AddressableAssetStatus.Loading)
            {
                await UniTask.WaitUntil(() => asset.status != AddressableAssetStatus.Loading);
            }
                    
            return asset.gameObject;
        }

        public bool AssetExists(string key)
        {
            return _addressablesAssets.ContainsKey(key);
        }

        public void Clear()
        {

            foreach (KeyValuePair<string, AddressableAsset> kv in _addressablesAssets)
            {

                // Release the instance of the Prefab from Addressables Cache
                // if we don't do that, the prefab will stay cached, but with a null value
                // because we are destroying it.

                if (kv.Value.status == AddressableAssetStatus.NotLoaded) continue;

                Addressables.ReleaseInstance(kv.Value.gameObject);
#if UNITY_EDITOR
                GameObject.DestroyImmediate(kv.Value.gameObject, true);
#else
                GameObject.Destroy(kv.Value.gameObject);
#endif
                kv.Value.gameObject = null;
                kv.Value.Release();

            }

            _addressablesAssets.Clear();

            Addressables.ClearResourceLocators();
        }


        string GetPlatformPrefix()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    return "StandaloneWindows64";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.OSXEditor:
                    return "StandaloneOSX";
                default:
                    return "StandaloneWindows64";
            }
        }

    }

    public enum AddressableAssetStatus
    {
        NotLoaded,
        Loading,
        Loaded,
        Failed,
    }

    public class AddressableAsset
    {
        public string catalogURI;
        public string address;
        public GameObject gameObject;
        public AddressableAssetStatus status { get; set; }
        public string addressablesURI;

        public AddressableAsset(string catalogURI, string address, string addressablesURI)
        {
            this.address = address;
            this.catalogURI = catalogURI;
            this.gameObject = null;
            this.addressablesURI = addressablesURI;
            status = AddressableAssetStatus.NotLoaded;
        }

        /// <summary>
        /// Loads the game object by key if it has not been loaded yet
        /// </summary>
        /// <returns></returns>
        public async UniTask Load()
        {
            status = AddressableAssetStatus.Loading;

#if (UNITY_WEBGL && !UNITY_EDITOR) || TEST_ADDRESSABLES
            try
            {
                string catalogURI = this.catalogURI;

                IResourceLocator resourceLocator = await Addressables.LoadContentCatalogAsync(catalogURI);

                Addressables.ResourceManager.InternalIdTransformFunc = (IResourceLocation location) =>
                {
                    return location.InternalId.Replace("REMOTE_HOST", addressablesURI);
                };

                IList<IResourceLocation> locations = null;

                if (resourceLocator.Locate(address, typeof(GameObject), out locations))
                {

                    if (locations.Count == 1)
                    {

                        gameObject = await Addressables.LoadAssetAsync<GameObject>(locations[0]);
                        if (gameObject == null)
                        {
                            Logging.LogError("[Content] Addressables IResourceLocator return null for " + address);
                        }
                        status = AddressableAssetStatus.Loaded;
                        return;
                    }
                    else Debug.LogError("Multiple or No GameObjects have been located at the " + address + " address. Please make sure there are no duplicate addressables in asset-service");
                }
                else Debug.LogError("No addressable GameObjects at the " + address + " address has been located");
            }
            catch (System.Exception e)
            {
                status = AddressableAssetStatus.Failed;
                // throw new System.Exception("Failed to load remote GameObject from " + URI + ". Eror: " + e.Message);

                Logging.Log("Failed to load remote Object: " + address + " from " + catalogURI + " with msg: " + e.Message);
            }
#else
                // In PlayMode in the Editor, we use the AssetDatabase directly, so we don't need to
                // load the catalog
                gameObject = await Addressables.LoadAssetAsync<GameObject>(address);
                status = Status.Loaded;
#endif
        }

        public void Release()
        {

        }
    }
}
