using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey;
using Cysharp.Threading.Tasks;
using HS;
using System.Linq;

namespace Odyssey
{
    public interface ISpawner
    {
        public void Init(IMomentumContext context);
        public void UnloadWorld();
        public UniTask SpawnWorldObject(WorldObject wo, bool animateAppear = false);
        public UniTask SpawnWorld();
        public UniTask SpawnEffect(Guid assetID, Vector3 position);
        public void DestoryWorldObject(WorldObject wo);

        public Action<WorldObject> OnObjectSpawned { get; set; }
        public Action<WorldObject> OnBeforeObjectDestroyed { get; set; }
        public Action<GameObject> OnPlayerAvatarSpawned { get; set; }

    }

    public class Spawner : MonoBehaviour, ISpawner, IRequiresContext
    {
        public Action<WorldObject> OnObjectSpawned { get; set; }
        public Action<WorldObject> OnBeforeObjectDestroyed { get; set; }
        public Action<GameObject> OnPlayerAvatarSpawned { get; set; }
        public GameObject spawnFXPrefab;
        public Texture2D memeDefault;
        public Texture2D posterDefault;
        public Texture2D textScreenDefault;

        private Transform _decorationsContainer;
        IMomentumContext _c;
        readonly Dictionary<string, ObjectPool> _objectPools;

        public Spawner()
        {
            _objectPools = new Dictionary<string, ObjectPool>();
        }

        public void Init(IMomentumContext context)
        {
            this._c = context;
        }

        /// <summary>
        /// Spawns and setups the World avatar controller, skybox and decorations
        /// </summary>
        /// <param name="currentWorldDefinition"></param>
        /// <param name="anchorPoint"></param>
        /// <returns></returns>
        public async UniTask SpawnWorldDecorations(WorldDefinition currentWorldDefinition)
        {
            // get the controller
            var avatarAsset = await _c.Get<IWorldPrefabHolder>().GetAssetAsync(currentWorldDefinition.worldAvatarController.ToString());
            var avatarPrefab = (avatarAsset != null && !avatarAsset.name.Contains("missing")) ? avatarAsset : _c.Get<IWorldPrefabHolder>().GetMissingAvatarControllerFallback();

            // get the decorations
            if (_decorationsContainer == null)
            {
                var decorationsContainerGO = new GameObject();
                decorationsContainerGO.name = "World Decorations";
                _decorationsContainer = decorationsContainerGO.transform;
            }

            // get the skybox manager
            var skyboxAsset = await _c.Get<IWorldPrefabHolder>().GetAssetAsync(currentWorldDefinition.worldSkyboxController.ToString());

            if (skyboxAsset != null)
            {
                var skyboxGO = Instantiate(skyboxAsset, Vector3.zero, Quaternion.identity);
                skyboxGO.transform.parent = _decorationsContainer;
                _c.Get<IWorldData>().WorldDecorations.Add(skyboxGO);
            }

            if (currentWorldDefinition.worldDecorations != null)
            {
                for (int i = 0; i < currentWorldDefinition.worldDecorations.Count; i++)
                {
                    var decorationAsset = await _c.Get<IWorldPrefabHolder>().GetAssetAsync(currentWorldDefinition.worldDecorations[i].guid.ToString());
                    if (decorationAsset == null) continue;
                    var temp = Instantiate(decorationAsset, currentWorldDefinition.worldDecorations[i].position, new Quaternion(0, 0, 0, 1));
                    temp.transform.parent = _decorationsContainer;
                    FindAndAddEffectTriggers(currentWorldDefinition.worldDecorations[i].guid, temp, currentWorldDefinition.worldDecorations[i].guid);
                    _c.Get<IWorldData>().WorldDecorations.Add(temp);
                }
            }

            if (avatarPrefab != null)
            {
                var tempAvatarGO = Instantiate(avatarPrefab, null);

                // Add the third person controller to the Context
                ThirdPersonController thirdPersonController = tempAvatarGO.GetComponent<ThirdPersonController>();
                thirdPersonController.Settings = _c.Get<ISessionData>().ControllerSettings;

                _c.RegisterService<IThirdPersonController>(thirdPersonController, true);

                _c.Get<ISessionData>().WorldAvatarController = tempAvatarGO;
                _c.Get<ISessionData>().AvatarCamera = tempAvatarGO.GetComponentInChildren<Camera>();

                OnPlayerAvatarSpawned?.Invoke(tempAvatarGO);
            }

        }




        /// <summary>
        /// Spawn the full World including avatar,skybox,decorations and WorldObjects
        /// </summary>
        /// <returns></returns>
        public async UniTask SpawnWorld()
        {
            await SpawnWorldDecorations(_c.Get<ISessionData>().worldDefinition);
            _c.Get<IReactAPI>().SendLoadingProgress(40);
            await SpawnWorldObjects();
        }

        /// <summary>
        /// Spawn all World Objects in WorldData.WorldHierachy
        /// </summary>
        /// <returns></returns>
        async UniTask SpawnWorldObjects()
        {
            int numObjects = _c.Get<IWorldData>().WorldHierarchy.Count;

            Logging.Log("[Spawner] World Objects Count: " + numObjects);

            // calculate how much we should move the loading progress for every spawn object
            float startProgress = 40.0f;
            float endProgress = 80.0f;

            float progressOffset = (endProgress - startProgress) / (float)numObjects;

            float p = 0.0f;

            foreach (var wo in _c.Get<IWorldData>().WorldHierarchy.Values)
            {
                await SpawnWorldObject(wo, false);
                p += progressOffset;

                if (p > 2.0f)
                {
                    startProgress += p;
                    _c.Get<IReactAPI>().SendLoadingProgress(Mathf.RoundToInt(Mathf.Clamp(startProgress, 0.0f, endProgress)));
                    p = 0.0f;
                }

            }

            _c.Get<ISessionStats>().AddTime("WorldSpawn");

        }

        /// <summary>
        /// Spawns a single WorldObject
        /// </summary>
        /// <param name="wo"></param>
        /// <param name="animateAppear">The spawned object will animate it's vertical position, if you set this to true</param>
        /// <returns></returns>
        public async UniTask SpawnWorldObject(WorldObject wo, bool animateAppear = false)
        {
            if (wo.GO != null) return;

            await CreateAndSetupGO(wo, animateAppear);


        }

        /// <summary>
        /// Loads an effect asset and just instantiate it at position
        /// </summary>
        /// <param name="assetID"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public async UniTask SpawnEffect(Guid assetID, Vector3 position)
        {
            GameObject effectPrefab = await _c.Get<IWorldPrefabHolder>().GetAssetAsync(assetID.ToString());
            if (effectPrefab == null) return;
            Instantiate(effectPrefab, position, Quaternion.identity);
        }

        /// <summary>
        /// Destroy just the GameObject representation of a GameObject
        /// </summary>
        /// <param name="wo"></param>
        public void DestoryWorldObject(WorldObject wo)
        {
            if (wo.GO == null) return;
            OnBeforeObjectDestroyed?.Invoke(wo);
            var assetGuid = wo.assetGuid.ToString();
            if (_objectPools.ContainsKey(assetGuid))
            {
                var objectPool = _objectPools[assetGuid];
                objectPool.Destroy(wo.GO);
            }
            wo.GO = null;
        }

        /// <summary>
        /// Shows as simple particle effect at the location of a spawned World Object
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        async UniTask ShowSpawnEffectsAfterDelay(Vector3 location)
        {
            await UniTask.Delay(3000);
            // Dont instantiate a particle system which the user won't see
            if (_c.Get<ISessionData>().WorldAvatarController != null)
            {
                var userPosition = _c.Get<ISessionData>().WorldAvatarController.transform.position;

                if ((userPosition - location).sqrMagnitude > 10000.0f)
                {
                    return;
                }
            }

            Destroy(Instantiate(spawnFXPrefab, location, new Quaternion(0, 0, 0, 1)), 10);
        }

        /// <summary>
        /// Creates the actual GameObject represenation of WorldObject and initialize it
        /// </summary>
        /// <param name="worldObject"></param>
        /// <param name="animateAppear"></param>
        /// <returns></returns>
        async UniTask CreateAndSetupGO(WorldObject worldObject, bool animateAppear = false)
        {

            worldObject.state = WorldObjectState.DOWNLOADING_ASSET;

            var objectPool = await GetObjectPoolForAsset(worldObject.assetGuid.ToString());

            if (objectPool == null)
            {
                Debug.Log("Object Pool is null..");
                worldObject.state = WorldObjectState.NO_ASSET_FOUND;
                return;
            }

            // Calculate the initial position
            var spawnPosition = animateAppear ? worldObject.position + new Vector3(0, -200, 0) : worldObject.position;

            var goName = "Guid:" + worldObject.guid + " Name: " + worldObject.name;
            var worldObjectGO = objectPool.Instantiate(goName);
            worldObjectGO.transform.position = spawnPosition;
            worldObject.GO = worldObjectGO;

            FindAndAddEffectTriggers(worldObject.guid, worldObject.GO, worldObject.assetGuid);

            // Update spaces/structure related behaviours
            var structureDriver = worldObject.GetStructureDriver();

            if (structureDriver == null)
            {
                structureDriver = worldObject.GO.AddComponent<AlphaStructureDriver>();
            }

            var options = worldObjectGO.GetComponent<StructureOptions>();

            if (options != null)
            {
                if (options.preloadMemeTexture)
                {

                    TextureData td = null;
                    worldObject.textures.TryGetValue("meme", out td);

                    if (td != null)
                    {
                        _c.Get<ITextureService>().LoadMemeTextureForStructure(td.originalHash, worldObject);
                    }

                }

                structureDriver.LookAtParent = options.lookAtParent;

                worldObject.onlyHighQualityTextures = options.onlyHighQualityTextures;
                worldObject.alwaysUpdateTextures = options.alwaysUpdateTextures;

                if (options.alwaysUpdateTextures)
                {
                    _c.Get<IWorldData>().AlwaysUpdateTexturesList.Add(worldObject);
                }

            }

            structureDriver.guid = worldObject.guid;

            if (worldObject.parentGuid != null && worldObject.parentGuid != Guid.Empty)
            {
                var parentWorldObject = _c.Get<IWorldData>().Get(worldObject.parentGuid);

                if (parentWorldObject != null && parentWorldObject.GO != null)
                {
                    structureDriver.parentTransform = parentWorldObject != null ? parentWorldObject.GO.transform : null;
                }
                else
                {
                    Logging.Log("[Spawner] Parent " + worldObject.parentGuid + " not found or not spawned for object: " + worldObject.guid);
                }

            }

            structureDriver.InitBehaviours();

            structureDriver.FillTextureSlot("solution", textScreenDefault);
            structureDriver.FillTextureSlot("problem", textScreenDefault);
            structureDriver.FillTextureSlot("video", textScreenDefault);
            structureDriver.FillTextureSlot("description", textScreenDefault);
            structureDriver.FillTextureSlot("meme", memeDefault);
            structureDriver.FillTextureSlot("poster", posterDefault);

            // Set the LOD to the lowest by default
            structureDriver.SetLOD(3);

            if (options != null && options.randomRotation)
            {
                float yRotation = GetRotationFromGUID(structureDriver.guid);
                Vector3 currentRotation = worldObjectGO.transform.localEulerAngles;
                currentRotation.y += yRotation;
                worldObjectGO.transform.localEulerAngles = currentRotation;
            }


            // attach to the parent gameobject
            var parentOfCurrentObject = _c.Get<IWorldData>().Get(worldObject.parentGuid);
            if (parentOfCurrentObject != null && worldObject.GO != null && parentOfCurrentObject.GO != null)
            {
                worldObject.GO.transform.SetParent(parentOfCurrentObject.GO.transform);
            }

            if (animateAppear)
            {
                ShowSpawnEffectsAfterDelay(worldObject.position).Forget();
                worldObjectGO.transform.position = worldObject.position + new Vector3(0, -200, 0);
                _c.Get<IStructureMover>().MoveStructure(worldObject.GO.transform, worldObject.GO.transform.parent, worldObject.position, structureDriver.LookAtParent).Forget();
            }
            else
            {
                worldObjectGO.transform.position = worldObject.position;

                if (worldObjectGO.transform.parent != null && structureDriver.LookAtParent)
                {
                    worldObjectGO.transform.LookAt(new Vector3(worldObjectGO.transform.parent.position.x, worldObjectGO.transform.position.y, worldObjectGO.transform.parent.position.z));
                }
            }

            worldObject.state = WorldObjectState.SPAWNED;
            OnObjectSpawned?.Invoke(worldObject);
        }

        void FindAndAddEffectTriggers(Guid key, GameObject go, Guid assetID)
        {
            IEffectsTrigger[] effectsTriggers = go.GetComponents<IEffectsTrigger>();

            var effectsHandlers = _c.Get<IWorldData>().EffectsHandlers;

            for (var i = 0; i < effectsTriggers.Length; ++i)
            {
                if (!effectsHandlers.ContainsKey(key))
                {
                    effectsHandlers[key] = new List<IEffectsTrigger>();
                }

                effectsHandlers[key].Add(effectsTriggers[i]);
            }

        }

        /// <summary>
        /// Unload and destroy all Gameobject and deletes all WorldObjects
        /// It also clears all cached Prefabs and Addressables
        /// Used when you are switching to a new world or quitting the app
        /// </summary>
        public void UnloadWorld()
        {
            _c.Get<ISessionData>().AppPaused = true;
            Time.timeScale = 0f;

            foreach (KeyValuePair<Guid, WorldObject> obj in _c.Get<IWorldData>().WorldHierarchy)
            {
                DestoryWorldObject(obj.Value);
            }

            if (_c.Get<ISessionData>().WorldAvatarController != null)
            {
                GameObject.Destroy(_c.Get<ISessionData>().WorldAvatarController);
            }

            // clear decorations
            for (int i = 0; i < _c.Get<IWorldData>().WorldDecorations.Count; i++)
            {
                Destroy(_c.Get<IWorldData>().WorldDecorations[i]);
            }

            // clear asset pools
            foreach (ObjectPool pool in _objectPools.Values)
            {
                pool.Clear();
            }

            _objectPools.Clear();

            // Clear Content
            _c.Get<IAddressablesProvider>().Clear();

            Time.timeScale = 1f;
            _c.Get<ISessionData>().AppPaused = false;
        }

        // if have set random rotation as an option for the structure
        // we will use the GUID (which is random) as a base for rotation on a Y axis
        // so the rotation is random, but it is the same during the lifetime of the space
        private float GetRotationFromGUID(Guid guid)
        {
            string hexNum = guid.ToString().Substring(0, 3);
            int number = Convert.ToInt32(hexNum, 16);
            float angle = (float)number / 4095.0f * 360.0f;
            return angle;
        }

        async UniTask<ObjectPool> GetObjectPoolForAsset(string assetID)
        {
            if (!_objectPools.ContainsKey(assetID))
            {

                var prefab = await _c.Get<IWorldPrefabHolder>().GetAssetAsync(assetID);

                if (prefab == null)
                {
                    Logging.Log("[Spawner] Could not find asset from PrefabHolder: " + assetID);
                    return null;
                }

                // check if the asset has been downloaded by another request for the Pool and same asset,
                // while we are trying to retrieve that asset
                if (_objectPools.ContainsKey(assetID))
                {
                    return _objectPools[assetID];
                }

                _objectPools.Add(assetID, new ObjectPool(prefab));
            }

            return _objectPools[assetID];
        }
    }
}

