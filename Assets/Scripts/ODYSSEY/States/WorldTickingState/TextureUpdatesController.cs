using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey;
using Odyssey.Networking;
using Cysharp.Threading.Tasks;
using System;

namespace Odyssey
{
    public class TextureUpdatesController : StateController
    {
        public TextureUpdatesController(IMomentumContext context) : base(context)
        {
        }

        public override void OnEnter()
        {
            _c.Get<ITextureService>().TextureDownloaded_Event += OnTextureDownloaded;
            _c.Get<IWorldDataService>().StructureTextureUpdated += StructureTextureUpdated;
        }

        public override void OnExit()
        {
            _c.Get<ITextureService>().TextureDownloaded_Event -= OnTextureDownloaded;
            _c.Get<IWorldDataService>().StructureTextureUpdated -= StructureTextureUpdated;
        }
        void StructureTextureUpdated(Guid guid, string label, string newHash)
        {
            WorldObject wo = _c.Get<IWorldData>().Get(guid);

            if (wo == null) return;

            float distance = (wo.position - _c.Get<ISessionData>().AvatarCamera.transform.position).sqrMagnitude;
            int lodLevel = _c.Get<ILODSystem>().GetLODLevelForDistance(distance);

            if (lodLevel > 0) return;

            if (label == "meme")
            {
                SendMemeEvent(guid, newHash).Forget();
            }
            else if (label == "poster")
            {
                SendPosterEvent(guid, newHash).Forget();
            }
            else
            {

            }
        }

        // Set the texture to the texture slots, once it is downloaded
        void OnTextureDownloaded(WorldObject wo, string label, CachedTexture texture)
        {
            float aspectRatio = 1;

            if (texture.width != 0 && texture.height != 0)
            {
                aspectRatio = (float)texture.width / (float)texture.height;
            }

            _c.Get<IMomentumAPI>().PublishTextureUpdate(wo.guid, label, texture.texReference, aspectRatio);

        }

        async UniTask SendPosterEvent(Guid key, string posterHashURL)
        {
            await UniTask.Delay(1000);

            try
            {
                var posterTexture = await _c.Get<IRendermanService>().DownloadTexture(posterHashURL, RendermanTextureSize.s5);

                Texture2D posterImage = posterTexture as Texture2D;

                WorldObject wo = _c.Get<IWorldData>().Get(key);

                if (wo != null && wo.GO != null)
                {
                    _c.Get<IEffectsService>().PosterUploadedFX(wo.GO.transform.position, posterImage);
                }
            }
            catch (Exception ex)
            {
                Logging.Log("[WorldEvents] Could not download poster texture." + ex.Message);
            }

        }


        async UniTask SendMemeEvent(Guid key, string hash)
        {

            Debug.Log("Meme event: " + key + " h: " + hash);
            await UniTask.Delay(1000);

            try
            {
                var memeTexture = await _c.Get<IRendermanService>().DownloadTexture(hash, RendermanTextureSize.s5);

                Texture2D memeImage = memeTexture as Texture2D;

                WorldObject wo = _c.Get<IWorldData>().Get(key);

                if (wo != null && wo.GO != null)
                {
                    _c.Get<IEffectsService>().MemeUploadedFX(wo.GO.transform.position, memeImage);
                }
            }
            catch (Exception ex)
            {
                Logging.Log("[WorldEvents] Could not download meme texture." + ex.Message);
            }

        }
    }
}