using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Odyssey.Networking;
using UnityEngine;

namespace Odyssey
{
    public interface IWorldDataService
    {
        public Action<Guid, string, string> StructureTextureUpdated { get; set; }
        public void Init(IMomentumContext context);
        public WorldDefinition CreateWorldDefinitionFromMsg(PosBusSetWorldMsg m);
        public WorldObject AddWorldObject(ObjectMetadata metaData);
        public void UpdatePositionForObject(Guid objectId, Vector3 position);
        public void RemoveWorldObject(Guid guid);
        public void UpdateObjectTexture(WorldObject wo, string textureLabel, string newTextureHash);
        public void UpdateObjectTextures(Guid guid, TextureMetadata[] textures);
        public UniTask GetWorldList();
        public bool CanAccessObject(Guid guid);
        public void UpdatePermissionsForObject(Guid guid, int privacyValue);
        public void UpdateObjectStrings(WorldObject wo, StringMetadata[] strings);
        public void ClearAll();
    }

    public class WorldDataService : IWorldDataService, IRequiresContext
    {
        IMomentumContext _c;

        public Action<Guid, string, string> StructureTextureUpdated { get; set; }

        public void Init(IMomentumContext context)
        {
            _c = context;
        }

        public WorldDefinition CreateWorldDefinitionFromMsg(PosBusSetWorldMsg m)
        {
            WorldDefinition worldDefinition = new WorldDefinition();

            worldDefinition.LOD1Distance = m.lodDistances[0];
            worldDefinition.LOD2Distance = m.lodDistances[1];
            worldDefinition.LOD3Distance = m.lodDistances[2];

            worldDefinition.worldAvatarController = m.avatarControllerID;
            worldDefinition.worldGuid = m.worldID;
            worldDefinition.worldSkyboxController = m.skyboxControllerID;

            worldDefinition.worldDecorations = new List<WorldDecoration>();

            if (m.decorations != null)
            {
                for (var i = 0; i < m.decorations.Length; ++i)
                {
                    DecorationMetadata metaData = m.decorations[i];
                    worldDefinition.worldDecorations.Add(
                        new WorldDecoration(metaData.assetID, metaData.position, metaData.rotation)
                        );
                }
            }

            return worldDefinition;
        }

        public WorldObject AddWorldObject(ObjectMetadata metaData)
        {
            if (_c.Get<IWorldData>().WorldHierarchy.ContainsKey(metaData.objectId))
            {
                //Logging.Log("[WorldDataService] Trying to add an object twice: " + metaData.objectId.ToString());
                return null;
            }

            WorldObject newObject = new WorldObject();

            newObject.guid = metaData.objectId;
            newObject.position = metaData.position;
            newObject.parentGuid = metaData.parentId;
            newObject.assetGuid = metaData.assetType;
            newObject.name = metaData.name;
            newObject.privateMode = 0;
            newObject.showOnMiniMap = metaData.isMinimap;
            newObject.uiAssetGuid = metaData.infoUIType;

            newObject.textlabels["name"] = new TextLabelData()
            {
                label = "name",
                text = newObject.name
            };

            //AddDefaultTexturesToObject(newObject);

            _c.Get<IWorldData>().WorldHierarchy.Add(newObject.guid, newObject);
            _c.Get<ILODSystem>().AddToLODCalculation(newObject);

            return newObject;
        }

        void AddDefaultTexturesToObject(WorldObject wo)
        {
            wo.textures["meme"] = new TextureData()
            {
                label = "meme",
                originalHash = _c.Get<ITextureService>().DefaultMemeTextureHash
            };

            wo.textures["poster"] = new TextureData()
            {
                label = "poster",
                originalHash = _c.Get<ITextureService>().DefaultPosterTextureHash
            };

            wo.textures["video"] = new TextureData()
            {
                label = "video",
                originalHash = _c.Get<ITextureService>().DefaultVideoTextureHash
            };

            wo.textures["description"] = new TextureData()
            {
                label = "description",
                originalHash = _c.Get<ITextureService>().DefaultTextureHash
            };

            wo.textures["problem"] = new TextureData()
            {
                label = "problem",
                originalHash = _c.Get<ITextureService>().DefaultTextureHash
            };

            wo.textures["solution"] = new TextureData()
            {
                label = "solution",
                originalHash = _c.Get<ITextureService>().DefaultTextureHash
            };

            wo.textures["texture"] = new TextureData()
            {
                label = "texture",
                originalHash = _c.Get<ITextureService>().DefaultTextureHash
            };

            wo.texturesDirty = true;
        }

        public void UpdatePositionForObject(Guid objectId, Vector3 newPosition)
        {

            WorldObject wo = _c.Get<IWorldData>().WorldHierarchy[objectId];

            // Remove from the LOD Octree and add it again with the new Position
            _c.Get<ILODSystem>().RemoveFromLODCalculation(wo);
            wo.position = newPosition;
            _c.Get<ILODSystem>().AddToLODCalculation(wo);

            _c.Get<IStructureMover>().MoveStructure(wo.GO.transform, wo.GO.transform.parent, newPosition).Forget();
        }

        public void RemoveWorldObject(Guid guid)
        {
            if (!_c.Get<IWorldData>().WorldHierarchy.ContainsKey(guid)) return;
            WorldObject wo = _c.Get<IWorldData>().WorldHierarchy[guid];
            _c.Get<ISpawner>().DestoryWorldObject(_c.Get<IWorldData>().WorldHierarchy[guid]);
            _c.Get<ITextureService>().UnloadAllTexturesForObject(wo);
            _c.Get<ILODSystem>().RemoveFromLODCalculation(wo);
            _c.Get<IWorldData>().WorldHierarchy.Remove(guid);
        }

        public void UpdateObjectTextures(Guid guid, TextureMetadata[] textures)
        {
            if (!_c.Get<IWorldData>().Exists(guid)) return;

            WorldObject wo = _c.Get<IWorldData>().Get(guid);
            for (var i = 0; i < textures.Length; ++i)
            {
                UpdateObjectTexture(wo, textures[i].label, textures[i].data);
            }
        }

        public void UpdateObjectTexture(WorldObject wo, string textureLabel, string newTextureHash)
        {

            TextureData textureData = null;

            wo.textures.TryGetValue(textureLabel, out textureData);

            if (textureData == null)
            {
                textureData = new TextureData();
                textureData.label = textureLabel;
                textureData.originalHash = newTextureHash;

                wo.textures[textureLabel] = textureData;
            }
            else
            {
                if (textureData.originalHash == newTextureHash) return;

                // if we have updated the hash
                // release the last used lodhash from the texturecache
                if (textureData.state == TextureDataState.DOWNLOADED)
                    _c.Get<ITextureCache>().DecRefCount(textureData.lodHash); // Decreate the ref count for the old texture hash with the LOD

                // mark the new texture as NOTLOADED, so it can be downloaded on the next update
                textureData.state = TextureDataState.NOTLOADED;
                textureData.originalHash = newTextureHash;

                StructureTextureUpdated?.Invoke(wo.guid, textureLabel, newTextureHash);

            }

            wo.texturesDirty = true;
        }

        public void UpdateObjectStrings(WorldObject wo, StringMetadata[] strings)
        {
            for (var i = 0; i < strings.Length; ++i)
            {
                if (wo.textlabels.ContainsKey(strings[i].label))
                {
                    wo.textlabels[strings[i].label].text = strings[i].data;
                }
                else
                {
                    wo.textlabels[strings[i].label] = new TextLabelData()
                    {
                        label = strings[i].label,
                        text = strings[i].data
                    };
                }
            }
        }

        public async UniTask GetWorldList()
        {

            var worlds = await _c.Get<IBackendService>().GetWorldsList();

            if (worlds == null) return;

            for (int i = 0; i < worlds.data.Length; i++)
            {
                _c.Get<IWorldData>().WorldsList.Add(Guid.Parse(worlds.data[i]));
            }
        }

        public void UpdatePermissionsForObject(Guid guid, int privacyValue)
        {
            WorldObject wo = _c.Get<IWorldData>().Get(guid);

            if (wo == null) return;

            wo.privateMode = privacyValue;

            AlphaStructureDriver structureDriver = wo.GetStructureDriver();

            if (structureDriver == null) return;

            structureDriver.SetPrivacy(wo.privateMode > 0, wo.privateMode < 2);

        }

        public bool CanAccessObject(Guid guid)
        {
            WorldObject wo = _c.Get<IWorldData>().WorldHierarchy[guid];

            if (wo == null)
            {
                Logging.LogError("[PermissionManager] IsStructurePrivate asking for structure with gui: " + guid + " does not exists in the World Hierarchy");
                return false;
            }

            return wo.privateMode < 2;
        }

        public void ClearAll()
        {
            _c.Get<ISpawner>().UnloadWorld();

            foreach (KeyValuePair<Guid, WorldObject> obj in _c.Get<IWorldData>().WorldHierarchy)
            {
                _c.Get<ILODSystem>().RemoveFromLODCalculation(obj.Value);
            }

            _c.Get<IWorldData>().WorldHierarchy.Clear();

        }
    }

}
