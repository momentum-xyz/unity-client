using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey;
using Odyssey.Networking;
using System;
using Random = UnityEngine.Random;
using Cysharp.Threading.Tasks;


[Serializable]
public class EffectsMockData
{
    public string name;
    public string id;
}

[CreateAssetMenu]
public class KusamaMockData : MockData
{
    [Header("Kusama Settings")]
    public string worldCenterAssetID = "b2ef3600-9595-2743-ac9d-0a86c1a327a2";

    [Header("Validator Nodes")]
    public Vector3 validatorNodesCenter;
    public string validatorNodeAssetID;
    public int numValidatorNodes = 100;
    public float valCloudSpreadX = 100.0f;
    public float valCloudSpreadY = 50.0f;
    public float valCloudSpreadZ = 100.0f;
    public bool useSpiralCloud = false;
    public int numNodesPerRotations = 10;
    public float spiralMaxRadius = 1000.0f;
    public int spiralShiftElements = 100;
    public float spiralHeightOffsetPerNode = 0.1f;
    public Vector3 spiralStartPosition = Vector3.zero;
    public string[] validatorNodesAttributes;
    public string validatorNodeUIAsset = "";


    [Header("Entity Spaces")]
    public Vector3 entitySpacesCenter;
    public string entitiySpaceAssetID;
    public int numClaimedEntitySpaces = 10;
    public int numNodesAttachedToEntities = 10;
    public float entitySpacesRadius = 200.0f;
    public float entitySpacesChildsRadius = 100.0f;
    public float entityChildNodesYOffset = 30.0f;
    public bool entityUseSphericalPositioning = false;
    public float entitySphereRadius = 50.0f;
    public string enititySpaceUIAsset = "8e080029-1a37-4319-bc8a-e7d15e1f0eb1";


    [Header("Parachains")]
    public string parachainsAssetID;
    public int numParachains = 5;
    public float parachainsRadius = 100.0f;
    public string parachainUIAssetID;

    [Header("Pie in the Sky")]
    public string pieInTheSkyAssetID;
    public Vector3 pieInTheSkyPosition;

    [Header("RelayChain")]
    public string relayChainAssetID;
    public Vector3 relayChainPosition;

    [Header("ThinAir")]
    public string thinAirAssetID;
    public Vector3 thinAirPosition;

    [Header("World Lobby")]
    public string worldLobbyAssetID;
    public Vector3 worldLobbyPosition;
    public string worldLobbyUIAssetID;

    [Header("SideStage")]
    public string sideStageAssetID;
    public Vector3[] sideStagePositions;

    [Header("Block")]
    public string blockUIAssetID;
    public string blockAssetID;

    [Header("Effects Assets IDs")]
    public EffectsMockData[] effectsAssets;

    [Header("Transaction Core")]
    public string transactionCoreAssetID;
    public Vector3 transactionCorePosition;

    [Header("WorldEffect")]
    public string worldEffectsAssetID = "6846dba3-38b1-4540-a80d-4ba04af4111e";

    [Header("Clocks")]
    public string eraClocksAssetID = "";
    public string eventsClockAssetID = "3ff9f56d-263f-434a-bae7-15907823c64f";
    public Vector3 eventsClocksPosition = Vector3.zero;
    public Vector3 eraClocksPosition = Vector3.zero;

    IMomentumContext _c;
    List<Guid> validatorNodesGuids = new List<Guid>();
    List<MockSpaceData> operators = new List<MockSpaceData>();
    List<Guid> parachainsGuids = new List<Guid>();
    MockSpaceData _transactionCore;


    public override void Init(IMomentumContext context)
    {
        Debug.Log("Init Mock Data...");
        _c = context;

        spaces.Clear();
        validatorNodesGuids.Clear();
        MockSpaceData worldCenter = new MockSpaceData();
        worldCenter.ID = WorldID;
        worldCenter.position = Vector3.zero;
        worldCenter.parentID = "";
        worldCenter.name = "KusamaWorld";
        worldCenter.assetTypeID = worldCenterAssetID;

        spaces.Add(worldCenter);

        string entitySpacesID = "99999999-8888-1111-1111-1111111";

        float angleOffset = 360.0f / numClaimedEntitySpaces;

        operators.Clear();
        for (var i = 0; i < numClaimedEntitySpaces; ++i)
        {
            Vector3 position = entitySpacesCenter;

            if (entityUseSphericalPositioning)
            {
                position += FibSphere(i, numClaimedEntitySpaces, entitySphereRadius);
            }
            else
            {

                position.x += entitySpacesRadius * Mathf.Cos(angleOffset * i * Mathf.Deg2Rad);
                position.z += entitySpacesRadius * Mathf.Sin(angleOffset * i * Mathf.Deg2Rad);
            }


            MockSpaceData sd = new MockSpaceData();
            sd.name = "EntitySpaces " + i.ToString();
            sd.ID = entitySpacesID + i.ToString("D5");
            sd.assetTypeID = entitiySpaceAssetID;
            sd.position = position;
            sd.parentID = worldCenter.ID;
            sd.uiAssetID = enititySpaceUIAsset;
            sd.showMinimap = true;

            spaces.Add(sd);

            operators.Add(sd);

            string entityChildsID = "99999999-8888-1111-";
            float vAngleOffset = 360.0f / numNodesAttachedToEntities;


            int randomNumNodes = Random.Range(2, numNodesAttachedToEntities);

            // Validator nodes
            for (var j = 0; j < randomNumNodes; ++j)
            {
                Vector3 vposition = sd.position;

                vposition += FibSphere(j, randomNumNodes, entitySpacesChildsRadius);

                //   vposition.y += entityChildNodesYOffset;
                //  vposition.x += entitySpacesChildsRadius * Mathf.Cos(vAngleOffset * j * Mathf.Deg2Rad);
                //   vposition.z += entitySpacesChildsRadius * Mathf.Sin(vAngleOffset * j * Mathf.Deg2Rad);

                MockSpaceData vsd = new MockSpaceData();
                vsd.name = "ValidatorChild " + i.ToString();
                vsd.ID = entityChildsID + j.ToString("D4") + "-" + i.ToString("D12");
                vsd.assetTypeID = validatorNodeAssetID;
                vsd.position = vposition;
                vsd.parentID = sd.ID;
                vsd.uiAssetID = validatorNodeUIAsset;
                //vsd.showMinimap = true;
                validatorNodesGuids.Add(Guid.Parse(vsd.ID));
                spaces.Add(vsd);
            }


        }


        // Validator Nodes
        string validatorsID = "99999999-9999-1111-1111-1111111";
        for (var i = 0; i < numValidatorNodes; ++i)
        {
            MockSpaceData sd = new MockSpaceData();
            sd.name = "Validator " + i.ToString();
            sd.ID = validatorsID + i.ToString("D5");
            sd.assetTypeID = validatorNodeAssetID;

            if (useSpiralCloud)
            {
                sd.position = validatorNodesCenter + GetPositionOnSpiral(i + spiralShiftElements, spiralMaxRadius, numNodesPerRotations, numValidatorNodes + spiralShiftElements);
            }
            else
            {
                sd.position = validatorNodesCenter + GetRandomPositionInCube(valCloudSpreadX, valCloudSpreadY, valCloudSpreadZ);
            }

            sd.uiAssetID = validatorNodeUIAsset;
            sd.parentID = worldCenter.ID;
            validatorNodesGuids.Add(Guid.Parse(sd.ID));
            spaces.Add(sd);
        }

        parachainsGuids.Clear();
        // Parachains
        string parachainsID = "99999999-6666-1111-1111-1111111";

        float paraAngleOffset = 360.0f / numParachains;
        for (var i = 0; i < numParachains; ++i)
        {
            Vector3 position = Vector3.zero;
            position.x += parachainsRadius * Mathf.Cos(paraAngleOffset * i * Mathf.Deg2Rad);
            position.z += parachainsRadius * Mathf.Sin(paraAngleOffset * i * Mathf.Deg2Rad);

            MockSpaceData sd = new MockSpaceData();
            sd.name = "Parachain " + i.ToString();
            sd.ID = parachainsID + i.ToString("D5");
            sd.assetTypeID = parachainsAssetID;
            sd.position = position;
            sd.parentID = worldCenter.ID;
            sd.uiAssetID = parachainUIAssetID;
            sd.showMinimap = true;

            parachainsGuids.Add(Guid.Parse(sd.ID));

            spaces.Add(sd);
        }




        // Relay Chain
        MockSpaceData rl = new MockSpaceData();
        rl.position = relayChainPosition;
        rl.assetTypeID = relayChainAssetID;
        rl.parentID = worldCenter.ID;
        rl.name = "Relay Chain";
        rl.ID = "99999999-0001-1111-1111-111111111111";

        spaces.Add(rl);

        // Pie In The Sky
        MockSpaceData pie = new MockSpaceData();
        pie.position = pieInTheSkyPosition;
        pie.parentID = worldCenter.ID;
        pie.name = "Pie In The Sky";
        pie.ID = "99999999-0002-1111-1111-111111111111";
        pie.assetTypeID = pieInTheSkyAssetID;

        spaces.Add(pie);

        // World Lobby
        MockSpaceData wl = new MockSpaceData();
        wl.assetTypeID = worldLobbyAssetID;
        wl.position = worldLobbyPosition;
        wl.name = "World Lobby";
        wl.uiAssetID = worldLobbyUIAssetID;
        wl.ID = "99999999-0003-1111-1111-111111111111";
        wl.parentID = worldCenter.ID;
        wl.showMinimap = true;

        spaces.Add(wl);

        // Side Stage
        for (var i = 0; i < sideStagePositions.Length; ++i)
        {
            MockSpaceData st = new MockSpaceData();
            st.assetTypeID = sideStageAssetID;
            st.position = sideStagePositions[i];
            st.name = "Side Stage " + i.ToString();
            st.ID = "99999999-0004-111" + i.ToString() + "-1111-111111111111";
            st.parentID = wl.ID;
            st.uiAssetID = worldLobbyUIAssetID;
            st.showMinimap = true;
            spaces.Add(st);
        }


        // Transaction Core
        _transactionCore = new MockSpaceData();
        _transactionCore.assetTypeID = transactionCoreAssetID;
        _transactionCore.position = transactionCorePosition;
        _transactionCore.name = "Transaction Core";
        _transactionCore.ID = "99999999-0005-1111-1111-111111111111";
        _transactionCore.parentID = worldCenter.ID;
        spaces.Add(_transactionCore);

        // World Effects
        MockSpaceData wfx = new MockSpaceData();
        wfx.assetTypeID = worldEffectsAssetID;
        wfx.position = Vector3.zero;
        wfx.name = "World Effects";
        wfx.ID = "99999999-0006-1111-1111-111111111111";
        wfx.parentID = worldCenter.ID;
        spaces.Add(wfx);

        // Clocks
        MockSpaceData cl = new MockSpaceData();
        cl.assetTypeID = eventsClockAssetID;
        cl.position = eventsClocksPosition;
        cl.name = "Event Clocks";
        cl.ID = "99999999-0007-1111-1111-111111111111";
        cl.parentID = worldCenter.ID;
        spaces.Add(cl);

        // Era clocks
        MockSpaceData cl2 = new MockSpaceData();
        cl2.assetTypeID = eraClocksAssetID;
        cl2.position = eraClocksPosition;
        cl2.name = "Era Clocks";
        cl2.ID = "99999999-0008-1111-1111-111111111111";
        cl2.parentID = worldCenter.ID;
        spaces.Add(cl2);

        DoEventsSimulations().Forget();
        SendValidatorsStrings().Forget();
        SendOperatorsStrings().Forget();
        SendOtherStrings().Forget();
        SendTexturesData().Forget();
    }

    TextureMetadata[] GetDefaultTextures()
    {
        return new TextureMetadata[]
            {
                new TextureMetadata()
                {
                    data = "80091aee79612a8124dce3b4be348183",
                    label = "meme"
                },
                new TextureMetadata()
                {
                    data = "80091aee79612a8124dce3b4be348183",
                    label = "poster"
                },
                new TextureMetadata()
                {
                    data = "80091aee79612a8124dce3b4be348183",
                    label = "description"
                },
                new TextureMetadata()
                {
                    data = "80091aee79612a8124dce3b4be348183",
                    label = "video"
                }

            };
    }

    async UniTask SendTexturesData()
    {
        await UniTask.WaitUntil(() => _c.Get<ISessionData>().WorldIsTicking);
        // Events Clocks
        var textureMsg = new PosBusSetTexturesMsg()
        {
            objectID = Guid.Parse("99999999-0007-1111-1111-111111111111"),
            textures = new TextureMetadata[]
            {
                new TextureMetadata()
                {
                    data = "80091aee79612a8124dce3b4be348183",
                    label = "texture"
                }
            }
        };

        _c.Get<IPosBus>().OnPosBusMessage(textureMsg);

        // World Lobby
        var wlMsg = new PosBusSetTexturesMsg()
        {
            objectID = Guid.Parse("99999999-0003-1111-1111-111111111111"),
            textures = GetDefaultTextures()
        };

        _c.Get<IPosBus>().OnPosBusMessage(wlMsg);

        foreach (var en in operators)
        {
            var etx = new PosBusSetTexturesMsg()
            {
                objectID = Guid.Parse(en.ID),
                textures = GetDefaultTextures()
            };
            _c.Get<IPosBus>().OnPosBusMessage(etx);
        }

        foreach (var p in parachainsGuids)
        {
            var etx = new PosBusSetTexturesMsg()
            {
                objectID = p,
                textures = GetDefaultTextures()
            };
            _c.Get<IPosBus>().OnPosBusMessage(etx);
        }
    }

    async UniTask SendValidatorsStrings()
    {
        await UniTask.WaitUntil(() => _c.Get<ISessionData>().WorldIsTicking);
        for (var i = 0; i < validatorNodesGuids.Count; ++i)
        {
            var strMsg = new PosBusSetStringsMsg()
            {
                spaceID = validatorNodesGuids[i],
                strings = new StringMetadata[] {
                    new StringMetadata()
                    {
                        label = "line1",
                        data = "VALIDATOR NODE"
                    },
                    new StringMetadata()
                    {
                        label = "line2",
                        data = validatorNodesGuids[i].ToString()
                    }
                }
            };
            _c.Get<IPosBus>().OnPosBusMessage(strMsg);
        }

        for (var i = 0; i < parachainsGuids.Count; ++i)
        {
            var strMsg = new PosBusSetStringsMsg()
            {
                spaceID = parachainsGuids[i],
                strings = new StringMetadata[] {
                    new StringMetadata()
                    {
                        label = "line1",
                        data = "PARACHAIN "+i.ToString()
                    },
                    new StringMetadata()
                    {
                        label = "line2",
                        data = parachainsGuids[i].ToString()
                    }
                }
            };
            _c.Get<IPosBus>().OnPosBusMessage(strMsg);
        }
    }

    async UniTask SendOtherStrings()
    {
        await UniTask.WaitUntil(() => _c.Get<ISessionData>().WorldIsTicking);

        var lobbyUpdateMsg = new PosBusSetStringsMsg()
        {
            spaceID = Guid.Parse("99999999-0003-1111-1111-111111111111"),
            strings = new StringMetadata[]
            {
                new StringMetadata()
                {
                    label = "line1",
                    data = "World Lobby"
                }


            }
        };

    }

    async UniTask SendOperatorsStrings()
    {
        await UniTask.WaitUntil(() => _c.Get<ISessionData>().WorldIsTicking);

        for (var i = 0; i < operators.Count; ++i)
        {
            var strMsg = new PosBusSetStringsMsg()
            {
                spaceID = Guid.Parse(operators[i].ID),
                strings = new StringMetadata[] {
                    new StringMetadata()
                    {
                        label = "line1",
                        data = "OPERATOR"
                    },
                    new StringMetadata()
                    {
                        label = "line2",
                        data = operators[i].name
                    },
                    new StringMetadata()
                    {
                        label = "numbertop",
                        data = Random.Range(1,200).ToString()
                    },
                    new StringMetadata()
                    {
                        label = "numberbottom",
                        data = Random.Range(1,200).ToString()
                    },
                    new StringMetadata()
                    {
                        label = "numberleft",
                        data = Random.Range(1,200).ToString()
                    },
                    new StringMetadata()
                    {
                        label = "numberright",
                        data = Random.Range(1,200).ToString()
                    }
                }
            };
            _c.Get<IPosBus>().OnPosBusMessage(strMsg);
        }
    }

    async UniTask DoEventsSimulations()
    {
        await UniTask.WaitUntil(() => _c.Get<ISessionData>().WorldIsTicking);

        SimulateRewards().Forget();

        while (true)
        {
            await UniTask.Delay(6000);
            SimulateBlock().Forget();
            await SimulateValidatorNodesStateChanges();
        }
    }

    public Vector3 GetRandomPositionInCube(float x, float y, float z)
    {
        return new Vector3(Random.Range(-1.0f, 1.0f) * x, Random.Range(-1.0f, 1.0f) * y, Random.Range(-1.0f, 1.0f) * z);
    }

    public Vector3 GetPositionOnSpiral(int idx, float maxRadius, int numSpacesPerRotation, int objectCount)
    {
        float numRotations = (float)objectCount / (float)numSpacesPerRotation;
        float maxAngle = numRotations * 360.0f;
        float angleOffset = maxAngle / objectCount;
        float angle = angleOffset * idx + 32.0f;

        float radius = (angle / maxAngle) * maxRadius;

        // float radius = maxRadius;

        Vector3 position = Vector3.zero;

        position.x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        position.z = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        position.y = spiralHeightOffsetPerNode * idx;

        return position;
    }

    public Vector3 GetSphereSurfaceRandomPosition(float radius)
    {
        float a1 = Random.Range(0, 180.0f);
        float a2 = Random.Range(0, 360.0f);

        Vector3 p = Vector3.zero;

        p.x = radius * Mathf.Sin(a1 * Mathf.Deg2Rad) * Mathf.Cos(a2 * Mathf.Deg2Rad);
        p.y = radius * Mathf.Sin(a1 * Mathf.Deg2Rad) * Mathf.Sin(a2 * Mathf.Deg2Rad);
        p.z = radius * Mathf.Cos(a1 * Mathf.Deg2Rad);

        return p;
    }

    public void AddBlock()
    {
        var msg = new PosBusAddStaticObjectsMsg();

        Guid blockGuid = GetRandomGUID();

        msg.objects = new ObjectMetadata[]
        {
            new ObjectMetadata()
            {
                objectId = blockGuid,
                name = "Block",
                assetType = Guid.Parse(blockAssetID),
                position = Vector3.zero,
                parentId = Guid.Parse(WorldID),
                infoUIType = Guid.Parse(blockUIAssetID)
            }
        };

        _c.Get<IPosBus>().OnPosBusMessage?.Invoke(msg);

        var attMsg = new PosBusSetStringsMsg()
        {
            spaceID = blockGuid,
            strings = new StringMetadata[]
            {
                new StringMetadata()
                {
                    label="line1",
                    data="BLOCK"
                },
                new StringMetadata()
                {
                    label="line2",
                    data="123456789100212"
                },
            }
        };
        _c.Get<IPosBus>().OnPosBusMessage(attMsg);


    }

    public void AddUser()
    {
        var posMsg = new PosBusPosMsg();
        posMsg.position = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
        posMsg.userId = Guid.NewGuid();

        _c.Get<IPosBus>().OnPosBusMessage(posMsg);
    }

    public void DeleteObject(string GUID)
    {
        var msg = new PosBusRemoveStaticObjectsMsg();
        msg.objectIds = new Guid[] { Guid.Parse(GUID) };

        _c.Get<IPosBus>().OnPosBusMessage(msg);
    }

    public void TransitionEffect(Guid emitter, Guid source, Guid destination, Guid assetID, int effectType)
    {
        var msg = new PosBusTransitionalBridgingEffectOnObjectMsg(emitter, source, destination, (uint)effectType);

        _c.Get<IPosBus>().OnPosBusMessage(msg);
    }

    public void StaticEffect(Guid emitter, Guid guid, Guid assetID, int effectType)
    {
        var msg = new PosBusTransitionalEffectOnObjectMsg(emitter, guid, (uint)effectType);
        _c.Get<IPosBus>().OnPosBusMessage(msg);
    }

    public async UniTask SimulateValidatorNodesStateChanges()
    {

        int numValidators = Random.Range(40, 80);

        for (var i = 0; i < numValidators; ++i)
        {
            Guid rv = validatorNodesGuids[Random.Range(0, validatorNodesGuids.Count)];

            PosBusSetAttributesMsg msg = new PosBusSetAttributesMsg();
            msg.spaceID = rv;
            msg.attributes = new AttributeMetadata[]
            {
                    new AttributeMetadata()
                    {
                        label = validatorNodesAttributes[Random.Range(0,validatorNodesAttributes.Length)],
                        attribute = Random.Range(0,10) > 5 ? 1 : 0
                    }
            };

            _c.Get<IPosBus>().OnPosBusMessage(msg);

        }

    }

    public async UniTask SimulateRewards()
    {
        Guid rewardsGuid = Guid.Parse("99999999-0002-1111-1111-111111111111");
        float showerAt = 60.0f;
        float t = 0.0f;
        while (true)
        {
            await UniTask.Delay(5000);
            t += 5;

            if (t >= 60.0f)
            {
                t = 0.0f;
                // Shower
                var showerMsg = new PosBusTransitionalEffectOnObjectMsg(rewardsGuid, rewardsGuid, 301);
                _c.Get<IPosBus>().OnPosBusMessage(showerMsg);

                for (var i = 0; i < 100; ++i)
                {
                    Guid randomNode = validatorNodesGuids[Random.Range(0, validatorNodesGuids.Count)];
                    var rewardsEffectMsg = new PosBusTransitionalBridgingEffectOnObjectMsg(rewardsGuid, rewardsGuid, randomNode, 302);
                    _c.Get<IPosBus>().OnPosBusMessage(rewardsEffectMsg);
                }

            }

            var attrMsg = new PosBusSetAttributesMsg();
            attrMsg.attributes = new AttributeMetadata[]
            {
                new AttributeMetadata()
                {
                    attribute = Mathf.RoundToInt(t / showerAt * 100.0f),
                    label = "rewardsamount"
                }
            };
            attrMsg.spaceID = rewardsGuid;
            _c.Get<IPosBus>().OnPosBusMessage(attrMsg);
        }
    }


    public async UniTask SimulateBlock()
    {
        // Add Block Object

        PosBusAddStaticObjectsMsg msg = new PosBusAddStaticObjectsMsg();
        Guid blockGuid = GetRandomGUID();
        msg.objects = new ObjectMetadata[]
        {
            new ObjectMetadata()
            {
                objectId = blockGuid,
                name = (1000000+Random.Range(99999,999999)+Random.Range(999,9999)).ToString(),
                assetType = Guid.Parse(blockAssetID),
                position = Vector3.zero,
                parentId = Guid.Parse(WorldID),
                infoUIType = Guid.Parse(blockUIAssetID)
            }
        };

        _c.Get<IPosBus>().OnPosBusMessage?.Invoke(msg);

        var attMsg = new PosBusSetStringsMsg()
        {
            spaceID = blockGuid,
            strings = new StringMetadata[]
            {
                new StringMetadata()
                {
                    label="line1",
                    data="BLOCK"
                },
                new StringMetadata()
                {
                    label="line2",
                    data="123456789100212"
                },
    }
        };
        _c.Get<IPosBus>().OnPosBusMessage?.Invoke(attMsg);
        await UniTask.Delay(1000);

        // Connect to random node
        Guid randomValidatorNode = validatorNodesGuids[Random.Range(0, validatorNodesGuids.Count)];

        TransitionEffect(blockGuid, blockGuid, randomValidatorNode, Guid.Empty, 10);

        await UniTask.Delay(35000);
        StaticEffect(blockGuid, blockGuid, Guid.Empty, 11);

        await UniTask.Delay(3000);

        PosBusRemoveStaticObjectsMsg removeMsg = new PosBusRemoveStaticObjectsMsg();
        removeMsg.objectIds = new Guid[] { blockGuid };
        _c.Get<IPosBus>().OnPosBusMessage(removeMsg);
    }

    public Guid GetRandomGUID()
    {
        return Guid.NewGuid();
    }


    private Vector3 FibSphere(int i, int n, float radius)
    {
        var k = i + .5f;

        var phi = Mathf.Acos(1f - 2f * k / n);
        var theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * k;

        var x = Mathf.Cos(theta) * Mathf.Sin(phi);
        var y = Mathf.Sin(theta) * Mathf.Sin(phi);
        var z = Mathf.Cos(phi);

        return new Vector3(x, y, z) * radius;
    }




}
