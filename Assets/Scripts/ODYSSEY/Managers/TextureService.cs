using Cysharp.Threading.Tasks;
using Odyssey.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

/// <summary>
/// Manages World Textures
/// </summary>

namespace Odyssey
{
    public interface ITextureService
    {
        public Action<WorldObject, string, CachedTexture> TextureDownloaded_Event { get; set; }
        public void UnloadAllTexturesForObject(WorldObject wo, bool skipMeme = false);
        public void LoadMemeTextureForStructure(string hash, WorldObject worldObject);

        public void UpdateTexturesForObject(WorldObject wo);
        public string DefaultTextureHash { get; }
        public string DefaultMemeTextureHash { get; }
        public string DefaultPosterTextureHash { get; }
        public string DefaultVideoTextureHash { get; }
    }

    public class TextureService : ITextureService, IRequiresContext
    {
        public Action<WorldObject, string, CachedTexture> TextureDownloaded_Event { get; set; }
        public string DefaultTextureHash { get; set; } = "a6d61b2bffb785299aa1eb26e1b540e9";
        public string DefaultMemeTextureHash { get; set; } = "69e2b342788fe70273c15b62f618ef22";
        public string DefaultPosterTextureHash { get; set; } = "53e9a2811a7a6cd93011a6df7c23edc7";
        public string DefaultVideoTextureHash { get; set; } = "1862dac5ee8441c3d5782b4063287aec";

        IMomentumContext _c;

        public Texture DefaultEmptyTexture
        {
            get
            {
                if (_defaultEmptyTexture == null)
                {
                    _defaultEmptyTexture = Resources.Load<Texture>("Textures/black") as Texture;
                }

                return _defaultEmptyTexture;
            }
        }
        private Texture _defaultEmptyTexture = null;

        public void Init(IMomentumContext context)
        {
            this._c = context;
        }


        /// <summary>
        /// Replaces all textures inside an object with a default empty one, but keeps the hashes untouched, so if you get close to that
        /// object again, the textures will load as expected
        /// </summary>
        /// <param name="wo"></param>
        /// <param name="skipMeme"></param>
        public void UnloadAllTexturesForObject(WorldObject wo, bool skipMeme = false)
        {
            if (!wo.texturesLoaded) return;

            AlphaStructureDriver alphaStructureDriver = wo.GetStructureDriver();

            foreach (KeyValuePair<string, TextureData> tsKV in wo.textures)
            {
                TextureData ts = tsKV.Value;

                if (ts.label == "meme" && skipMeme) continue;

                if (ts.state == TextureDataState.DOWNLOADED) _c.Get<ITextureCache>().DecRefCount(ts.lodHash);

                _c.Get<IMomentumAPI>().PublishTextureUpdate(wo.guid, ts.label, (Texture2D)DefaultEmptyTexture, 1.0f);

                ts.state = TextureDataState.NOTLOADED;
            }

            wo.texturesDirty = true;
            wo.texturesLoaded = false;
        }

        /// <summary>
        /// Downloads and sets just the texture with the label "meme"
        /// ! Used only for the Alpha world !
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="wo"></param>
        public void LoadMemeTextureForStructure(string hash, WorldObject wo)
        {
            TextureData td = wo.GetTextureDataForLabel("meme");

            if (td == null) return;

            UpdateTextureData(wo, td);
        }

        public void UpdateTexturesForObject(WorldObject wo)
        {
            foreach (KeyValuePair<string, TextureData> t in wo.textures)
            {
                TextureData td = t.Value;

                string currentLODHash = td.originalHash + wo.texturesLOD;

                // do we have a switch in the texture LOD
                bool switchingLOD = td.state == TextureDataState.DOWNLOADED && (td.lodHash != currentLODHash);

                // do we need to download the textures again
                bool needsTextureUpdate = (td.state == TextureDataState.NOTLOADED) || (td.lodHash != currentLODHash);

                if (switchingLOD)
                {
                    // if we already has downloaded an LOD version of this hash, remove the reference of the old one
                    _c.Get<ITextureCache>().DecRefCount(td.lodHash);
                }

                if (needsTextureUpdate)
                {
                    UpdateTextureData(wo, td);
                }

            }

            wo.texturesLoaded = true;
            wo.texturesDirty = false;

        }

        void UpdateTextureData(WorldObject wo, TextureData td)
        {

            td.lodHash = td.originalHash + wo.texturesLOD;
            td.state = TextureDataState.DOWNLOADED;

            if (!_c.Get<ITextureCache>().IsTextureCached(td.lodHash)) // NOT CACHED!
            {
                CachedTexture newWorldTextureData = _c.Get<ITextureCache>().AddTexture(null, td.lodHash, 0, 0, 0);
                _c.Get<ITextureCache>().IncRefCount(td.lodHash);

                DownloadTextureAndFillCache(wo, td, newWorldTextureData, GetSizeByTexturedLOD(wo.texturesLOD)).Forget();
            }
            else // CACHED
            {
                _c.Get<ITextureCache>().IncRefCount(td.lodHash);
                CachedTexture ct = _c.Get<ITextureCache>().GetTexture(td.lodHash);
                TextureDownloaded_Event?.Invoke(wo, td.label, ct);
            }
        }



        // We assume that no matter what, we are going to have a texture, if we fail to download the original one, we will use the default texture instead
        // but treat it as a new one and use a separate cache slot for consistency
        private async UniTask DownloadTextureAndFillCache(WorldObject wo, TextureData td, CachedTexture cachedTextureData, RendermanTextureSize size)
        {
            //Logging.Log("Downloading texture for: " + wo.name + " / " + td.label + " => " + td.lodHash);
            string currentLODHash = td.lodHash;

            Texture2D texture = (Texture2D)DefaultEmptyTexture;

            cachedTextureData.texReference = texture; // set an empty texture by default

            try
            {
                texture = await _c.Get<IRendermanService>().DownloadTexture(td.originalHash, size);

                // check if the data has changed while downloading
                if (currentLODHash != td.lodHash)
                {
                    texture = null;
                    return;
                }

                // check if somebody has downloaded this into the cache while we are downloading that texture
                CachedTexture alreadyExisting = _c.Get<ITextureCache>().GetTexture(td.lodHash);

                if (!alreadyExisting.downloaded)
                {
#if UNITY_2020
                    texture.Compress(false);
#endif
                    texture.name = td.lodHash;

                    // After the compression, set the texture to nonReadable
                    // so we don't keep it's pixel data inside Memory (this is a huge relief for the UnityHeap on WebGL)
                    texture.Apply(false, true);

                    cachedTextureData.width = texture.width;
                    cachedTextureData.height = texture.height;
                    cachedTextureData.texReference = texture;


                }
            }
            catch (Exception ex)
            {
                Logging.Log("[TextureService] Loading of texture: " + td.originalHash + " failed." + ex.Message);
            }

            cachedTextureData.downloaded = true;
            cachedTextureData.memorySize = Profiler.GetRuntimeMemorySizeLong(texture);

            TextureDownloaded_Event?.Invoke(wo, td.label, cachedTextureData);

            texture = null;
        }

        private void PopulateTextureToObjectTextureSlots(WorldObject wo, CachedTexture worldTextureData)
        {


        }

        private RendermanTextureSize GetSizeByTexturedLOD(int lod)
        {

            RendermanTextureSize size = RendermanTextureSize.s2;

            switch (lod)
            {
                case 0:
                    size = RendermanTextureSize.s5;
                    break;
                case 1:
                    size = RendermanTextureSize.s2;
                    break;
                case 2:
                    size = RendermanTextureSize.s2;
                    break;
            }

            return size;
        }
    }
}