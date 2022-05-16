using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityEngine.Networking;
using Odyssey.Networking;
using Odyssey;
using Cysharp.Threading.Tasks;

namespace Odyssey
{
    public interface IWispManager
    {
        public void Init(IMomentumContext context);
        public int GetWispsCount();
        public Dictionary<Guid, WispData> GetWisps();
        public WispData GetWispDataForGuid(Guid userGuid);
        public Vector3 GetWispPosition(Guid userGuid);
        public void SetWispsParticlesMaterial(Material mat);
        public void SetDefaultParticleMaterial();
        public void SetFullWispPrefab(GameObject prefab);
        public void SetFullWispSwitchDistance(float distance);
        public void StartRunning();
        public void Stop();
        public void InitWispsPrefabsPool();

        public void SetOptions(int maxParticleWisps, int maxFullWisps, float wispTimeout);
        public void Clear();

        public Action<WispData> OnWispAdded { get; set; }
        public Action<WispData> OnWispRemoved { get; set; }
    }

    public class WispManager : MonoBehaviour, IWispManager, IRequiresContext
    {
        [Header("Full Wisp Representation")]
        public GameObject defaultFullWispPrefab;
        public Material defaultParticlesMaterial;
        public GameObject fullWispPrefab;

        [Header("Move Speed")]
        public float normalSpeed = 32.0f;
        public float maxSpeed = 65.0f;

        [Header("Options")]
        public int maxiumParticles = 5000;                                     // used for particle pooling
        public int maximumFillWisps = 30;
        public float wispTimeout = 10.0f;
        public float distanceToFullWispSwitch = 30.0f;

        public Action<WispData> OnWispAdded { get; set; }
        public Action<WispData> OnWispRemoved { get; set; }

        // used to tell unity manager that this has loaded
        private Dictionary<Guid, WispData> wisps = new Dictionary<Guid, WispData>();
        private List<Guid> fullWispIDs = new List<Guid>();

        private ParticleSystem wispsParticleSystem;                                              // used for particle pooling
        private ParticleSystem.EmitParams emitterComponent;                                     // used for particle pooling    

        private int lastParticleIndex = 0;                                                      // used for particle pooling
        private ParticleSystem.Particle[] particles;                                             // particle storage

        private List<GameObject> wispPrefabPool;


        private Color invisibleColor = new Color(0f, 0f, 0f, 0f);
        private Color visibleColor = new Color(1f, 1f, 1f, 1f);

        private bool _isWispsParticleSystemDirty = false;
        private bool _processRecievedMessage = false;

        IMomentumContext _c;

        public void Init(IMomentumContext context)
        {
            this._c = context;

        }

        void OnEnable()
        {
            _c.Get<IPosBus>().OnPosBusMessage += OnPosBusMessage;
            InitParticleSystem();
        }

        void OnDisable()
        {
            _c.Get<IPosBus>().OnPosBusMessage -= OnPosBusMessage;
        }

        public void StartRunning()
        {
            _processRecievedMessage = true;
        }

        public void Stop()
        {

            _processRecievedMessage = false;
        }

        void OnPosBusMessage(IPosBusMessage msg)
        {
            switch (msg)
            {
                case PosBusPosMsg m:
                    ReceivedPositionMessage(m.userId, m.position);
                    break;
                case PosBusRemoveUserMsg m:
                    RemoveWisp(m.userId);
                    break;
            }
        }

        public void SetFullWispSwitchDistance(float distance)
        {
            this.distanceToFullWispSwitch = distance;
        }

        public void SetOptions(int maxParticleWisps, int maxFullWisps, float wispTimeout)
        {
            this.maxiumParticles = maxParticleWisps;
            this.maximumFillWisps = maxFullWisps;
            this.wispTimeout = wispTimeout;
        }

        public void SetDefaultParticleMaterial()
        {
            SetWispsParticlesMaterial(defaultParticlesMaterial);
        }

        void InitParticleSystem()
        {
            wispsParticleSystem = GetComponent<ParticleSystem>();

            if (particles == null || particles.Length < wispsParticleSystem.main.maxParticles)
            {
                particles = new ParticleSystem.Particle[wispsParticleSystem.main.maxParticles];
            }

            wispsParticleSystem.Play();
        }

        public void InitWispsPrefabsPool()
        {
            if (fullWispPrefab == null)
            {
                fullWispPrefab = defaultFullWispPrefab;
            }

            wispPrefabPool = new List<GameObject>();

            // spawn the pool
            for (int i = 0; i < maximumFillWisps; i++)
            {
                GameObject pooledFullWisp = Instantiate(fullWispPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1));
                pooledFullWisp.transform.parent = this.gameObject.transform;
                wispPrefabPool.Add(pooledFullWisp);
            }
        }

        void ClearPool()
        {
            for (var i = 0; i < wispPrefabPool.Count; ++i)
            {
#if UNITY_EDITOR
                DestroyImmediate(wispPrefabPool[i]);
#else
            Destroy(wispPrefabPool[i]);
#endif
            }
        }

        /// <summary>
        /// Updates the prefab that is used by the full wisp representation of the user
        /// </summary>
        /// <param name="prefab"></param>
        public void SetFullWispPrefab(GameObject prefab)
        {
            fullWispPrefab = prefab;
        }

        /// <summary>
        /// Updates the material of the particle system that handles the visualisation of the wisps when they are seen from far away
        /// </summary>
        /// <param name="mat"></param>
        public void SetWispsParticlesMaterial(Material mat)
        {
            ParticleSystemRenderer render = wispsParticleSystem.GetComponent<ParticleSystemRenderer>();
            render.sharedMaterial = mat;
        }

        public int GetWispsCount()
        {
            return wisps.Keys.Count;
        }

        public Dictionary<Guid, WispData> GetWisps()
        {
            return wisps;
        }


        /// <summary>
        /// Removes the wisp represenation of the user and releases the pooled gameobject for the next connected user/wisp
        /// </summary>
        /// <param name="userGuidToRemove"></param>

        void RemoveWisp(Guid userGuidToRemove)
        {
            if (!wisps.ContainsKey(userGuidToRemove)) return;

            var wispData = wisps[userGuidToRemove];

            particles[wispData.particleID].position = new Vector3(0, 0, 0);
            particles[wispData.particleID].startColor = invisibleColor;
            particles[wispData.particleID].startLifetime = -1f;

            _isWispsParticleSystemDirty = true;

            // clean up full wisp
            if (fullWispIDs.Contains(userGuidToRemove))
            {
                fullWispIDs.Remove(userGuidToRemove);
            }

            if (wisps[userGuidToRemove].fullWisp != null)
            {
                wisps[userGuidToRemove].fullWisp.SetActive(false);
            }

            wisps.Remove(userGuidToRemove);

            OnWispRemoved?.Invoke(wispData);
        }

        void Update()
        {
            if (!_c.Get<ISessionData>().WorldIsTicking || _c.Get<ISessionData>().AvatarCamera == null)
            {
                return;
            }

            UpdateExpiredWisps();
            MoveWispsParticles();

            if (_isWispsParticleSystemDirty)
            {
                wispsParticleSystem.SetParticles(particles);
                _isWispsParticleSystemDirty = false;
            }

            ProcessFullWisps();
        }

        /// <summary>
        /// Goes through all wisps and checks if the current player is close enough to see the Full Wisp representation of the Particle one
        /// </summary>
        void ProcessFullWisps()
        {
            Vector3 cameraPosition = _c.Get<ISessionData>().AvatarCamera.transform.position;

            foreach (KeyValuePair<Guid, WispData> wisp in wisps)
            {
                if (Vector3.Distance(wisp.Value.currentPosition, cameraPosition) < distanceToFullWispSwitch)
                {
                    // if we don't have a full wisp created, create one
                    if (wisp.Value.fullWisp == null)
                    {
                        GetAndUpdateNewFullWispFor(wisp.Key, wisp.Value);
                    }

                    if (wisp.Value.fullWisp != null)
                    {
                        wisp.Value.fullWisp.transform.position = wisp.Value.currentPosition;
                    }
                }
                else
                {
                    CleanFullWispFor(wisp.Key, wisp.Value);
                }

            }
        }

        /// <summary>
        /// Returns the Full wisp to the Pool and then shows the Particle represenation of it
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="wispData"></param>
        void CleanFullWispFor(Guid userID, WispData wispData)
        {
            if (wispData.fullWisp == null) return;

            // moved out of range

            // clean up full wisp
            if (fullWispIDs.Contains(userID))
            {
                fullWispIDs.Remove(userID);
            }

            int particleID = wispData.particleID;

            if (particleID >= 0 && particleID < particles.Length)
            {
                particles[particleID].startSize = 1f;
                particles[particleID].startColor = visibleColor;
                particles[particleID].position = wispData.currentPosition;
            }

            _isWispsParticleSystemDirty = true;

            wispData.fullWisp.SetActive(false);
            wispData.fullWisp = null;

        }

        /// <summary>
        /// Gets a new Full Wisps from the Pool and populate it with the user's data
        /// Then it hides the Particle represenation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="wispData"></param>
        void GetAndUpdateNewFullWispFor(Guid userID, WispData wispData)
        {
            GameObject fullWisp = GetFreeWispFromPool();
            fullWisp.SetActive(true);
            fullWisp.transform.position = wispData.currentPosition;
            fullWisp.name = wispData.name + " " + userID.ToString();

            FullWispManager wispManager = fullWisp.GetComponent<FullWispManager>();

            if (wispManager != null)
            {
                wispManager.guid = userID;
                wispManager.userID = userID.ToString();
                wispManager.wispName = wispData.name;
                wispManager.SetRole(0, userID.ToString());
            }

            int particleID = wispData.particleID;
            if (particleID >= 0 && particleID < particles.Length)
            {
                particles[particleID].startSize = 0f;
                particles[particleID].startColor = invisibleColor;
            }

            _isWispsParticleSystemDirty = true;
            fullWispIDs.Add(userID);

            wispData.wispManager = wispManager;
            wispData.fullWisp = fullWisp;
        }

        /// <summary>
        /// Goes through all wisps and see if some of them hasn't received an update for a while and remove them, accordingly
        /// </summary>
        void UpdateExpiredWisps()
        {
            float currentTime = Time.realtimeSinceStartup;
            List<Guid> timeoutWisps = new List<Guid>();

            Vector3 cameraPosition = _c.Get<ISessionData>().AvatarCamera.transform.position;

            foreach (KeyValuePair<Guid, WispData> wisp in wisps)
            {
                // check to see if wisp has been updated
                if (wisp.Value.lastUpdate + wispTimeout < Time.fixedTime)
                {
                    Logging.Log("[WispsManager] Wisp timeout: " + wisp.Key);
                    timeoutWisps.Add(wisp.Key);
                }
            }

            foreach (Guid wipsGuid in timeoutWisps)
            {
                RemoveWisp(wipsGuid);
            }

            timeoutWisps.Clear();
        }

        /// <summary>
        /// Update wisps positions
        /// </summary>
        void MoveWispsParticles()
        {
            foreach (KeyValuePair<Guid, WispData> wisp in wisps)
            {
                Vector3 currentPosition = wisp.Value.currentPosition;
                Vector3 targetPosition = wisp.Value.updatedPosition;

                float d = Vector3.Distance(currentPosition, targetPosition);

                if (d > 0.05f)
                {

                    float SPEED = 0.0f;

                    Vector3 direction = (targetPosition - currentPosition).normalized;
                    float distance = (currentPosition - targetPosition).sqrMagnitude;

                    if (d > 50.0f)
                    {
                        SPEED = maxSpeed;
                    }
                    else if (d > 1.0f)
                    {
                        SPEED = normalSpeed;
                    }
                    else
                    {
                        // slow down when we reach close distance
                        SPEED = normalSpeed * d;
                    }


                    Vector3 delta = direction * Time.deltaTime * SPEED;

                    if (delta.magnitude < d)
                    {
                        wisp.Value.currentPosition += delta;
                    }
                    else
                    {
                        wisp.Value.currentPosition += delta.normalized * d;
                    }

                    particles[wisp.Value.particleID].position = wisp.Value.currentPosition;

                    _isWispsParticleSystemDirty = true;
                }
            }

        }


        GameObject GetFreeWispFromPool()
        {
            for (int i = 0; i < wispPrefabPool.Count; i++)
            {
                if (wispPrefabPool[i].activeInHierarchy == false)
                {
                    return wispPrefabPool[i];
                }
            }
            return null;
        }

        public WispData GetWispDataForGuid(Guid userGuid)
        {
            WispData data = null;
            wisps.TryGetValue(userGuid, out data);

            return data;
        }

        void CreateWispParticle(WispData wispData)
        {
            if (_c.Get<ISessionData>().AvatarCamera == null || wispData == null) return;

            int newParticleID = -1;

            // wisp does not have an id, so needs one
            if (wispData.particleID == -1)
            {

                bool closeEnoughForFullWisp = Vector3.Distance(_c.Get<ISessionData>().AvatarCamera.transform.position, wispData.currentPosition) < distanceToFullWispSwitch;

                float particleSize = closeEnoughForFullWisp ? 0.0f : 1.0f;
                newParticleID = SpawnParticle(wispData.currentPosition, particleSize, new Vector3(0f, 0f, 0f), 0f);
                wispData.particleID = newParticleID;
            }
        }

        int SpawnParticle(Vector3 position, float size, Vector3 velocity, float angularVelocity)
        {
            lastParticleIndex++;

            if (lastParticleIndex >= maxiumParticles)
            {
                lastParticleIndex = 1;
            }

            emitterComponent.angularVelocity = angularVelocity;
            emitterComponent.position = position;
            emitterComponent.startSize = size;
            emitterComponent.velocity = velocity;
            emitterComponent.startLifetime = float.MaxValue;

            wispsParticleSystem.Emit(emitterComponent, 1);

            particles[lastParticleIndex].position = position;
            particles[lastParticleIndex].startSize = size;
            particles[lastParticleIndex].startColor = visibleColor;

            return lastParticleIndex;
        }



        // used by the notification system to find a wisp position
        public Vector3 GetWispPosition(Guid userGuid)
        {

            Vector3 position;

            if (userGuid == _c.Get<ISessionData>().UserID)
            {
                if (_c.Get<ISessionData>().WorldAvatarController == null)
                {
                    Logging.LogError("[WispManager] The global Avatar Controller is null!");
                    return Vector3.zero;
                }

                return _c.Get<ISessionData>().WorldAvatarController.transform.position;
            }

            foreach (GameObject wispPrefab in wispPrefabPool)
            {
                FullWispManager wispManager = wispPrefab.GetComponent<FullWispManager>();

                if (wispManager != null && wispManager.userID == userGuid.ToString())
                {
                    return wispPrefab.transform.position;
                }
            }

            if (wisps.ContainsKey(userGuid))
            {
                position = wisps[userGuid].currentPosition;
            }
            else
            {
                position = new Vector3(0f, 0f, 0f);
            }

            return position;
        }




        /// <summary>
        /// Process received position updates from the network (PosBus)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void ReceivedPositionMessage(Guid userId, Vector3 positionFromNetwork)
        {
            if (!_processRecievedMessage) return;

            Guid wispID = userId;

            // Do not process position updates from the current user
            if (wispID == _c.Get<ISessionData>().UserID) return;

            WispData wispData = null;

            wisps.TryGetValue(wispID, out wispData);

            if (wispData == null)
            {
                wispData = new WispData();
                wispData.guid = wispID;
                //wispData.uiAssetGuid = Guid.Parse("61465ad2-9cfb-456b-8cd9-d8ca3676d732");
                wispData.currentPosition = positionFromNetwork;
                wispData.particleID = -1;
                wispData.fullWisp = null;

                UpdateFullWispUserData(wispID, wispData).Forget();

                CreateWispParticle(wispData);

                Debug.Log("Adding new user: " + userId);

                OnWispAdded?.Invoke(wispData);
            }

            wispData.lastUpdate = Time.fixedTime;
            wispData.updatedPosition = positionFromNetwork;

            wisps[wispID] = wispData;

        }

        public void Clear()
        {

            _processRecievedMessage = false;

            // Put all Guids in a separate list, because 
            // we will have problems when we iterate and remove from the same Dictionary
            List<Guid> guidsToRemove = new List<Guid>();
            foreach (Guid g in wisps.Keys)
            {
                guidsToRemove.Add(g);
            }

            for (var i = 0; i < guidsToRemove.Count; i++)
            {
                RemoveWisp(guidsToRemove[i]);
            }

            wisps.Clear();

            _isWispsParticleSystemDirty = true;

            ClearPool();

            fullWispPrefab = null;
        }

        async UniTask UpdateFullWispUserData(Guid userID, WispData wispData)
        {
            try
            {
                var userData = await _c.Get<IBackendService>().GetUserData(userID.ToString());
                wispData.name = userData.name;

                if (wispData.fullWisp != null && wispData.wispManager != null)
                {
                    wispData.wispManager.SetName(userData.name);
                }
            }
            catch (Exception e)
            {
                Logging.Log("[WispManager] Could not download the data for user: " + userID + " (" + e.Message + ")");
                wispData.name = "Unknown";
            }


        }
    }

    // used to hold alpha wisp data
    public class WispData : IInfoUIHovarable
    {
        public Vector3 PositionInWorld { get { return currentPosition; } set { } }
        public Guid uiAssetGuid { get; set; }
        public Guid guid { get; set; }
        public Vector3 currentPosition;
        public Vector3 updatedPosition;
        public float lastUpdate;
        public bool processed;
        public int particleID;
        public GameObject fullWisp;
        public string name;
        public FullWispManager wispManager = null;
    }
}