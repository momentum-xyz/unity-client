using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Odyssey
{

    public interface ITextureCache
    {
        public CachedTexture AddTexture(Texture2D texRef, string hash, int w, int h, long size);
        public bool IsTextureCached(string hash);
        public CachedTexture GetTexture(string hash);
        public void IncRefCount(string hash);

        public void DecRefCount(string hash);

        public void DebugTextures();
        public void Update();

        public void Clear();
    }

    public class CachedTexture
    {
        public string hash;
        public string ownerGuid; // Guid of the structure containing the texture
        public int width;
        public int height;
        public int refCount = 0;
        public long memorySize;
        public Texture2D texReference;
        public bool markForDeletion = false;
        public float markedForDeletionTime = 0.0f;
        public bool downloaded = false;
    }

    public class TextureCache : ITextureCache
    {
        Dictionary<string, CachedTexture> textures = new Dictionary<string, CachedTexture>();
        float runUpdatesAt = 5.0f;
        float deleteAfterDelayOf = 25.0f;
        float updateTimer = 0.0f;

        public int TexturesCount => textures.Count;

        public CachedTexture AddTexture(Texture2D texRef, string hash, int w, int h, long size)
        {
           // Debug.Log("Adding to cache: " + hash);

            CachedTexture wt = null;

            textures.TryGetValue(hash, out wt);

            if (wt == null)
            {
                textures[hash] = new CachedTexture()
                {
                    hash = hash,
                    width = w,
                    height = h,
                    refCount = 0,
                    memorySize = size,
                    texReference = texRef,
                    downloaded = false
                };
            }

            return textures[hash];

        }

        public bool IsTextureCached(string hash)
        {
            CachedTexture search = GetTexture(hash);
            if (search != null && search.downloaded) return true;

            return false;
        }

        public void IncRefCount(string hash)
        {
            //Debug.Log("Inc Reference: " + hash);

            CachedTexture wt = null;
            textures.TryGetValue(hash, out wt);

            if (wt == null) return;

            wt.refCount++;
            wt.markForDeletion = false;
        }

        /// <summary>
        /// Release will decrease the reference count of a texture with 1
        /// and if we reach zero, will remove the reference from the textures dictionary
        /// so it will be released from memory
        /// </summary>
        /// <param name="hash"></param>
        public void DecRefCount(string hash)
        {
           // Debug.Log("Dec Reference: " + hash);

            CachedTexture wt = null;
            textures.TryGetValue(hash, out wt);

            if (wt == null) return;

            wt.refCount--;

            if (wt.refCount <= 0)
            {
                //Debug.Log("Mark for deletion..." + hash);
                wt.markForDeletion = true;
                wt.markedForDeletionTime = Time.fixedTime;
            }
        }

        public CachedTexture GetTexture(string hash)
        {
            CachedTexture wt = null;
            textures.TryGetValue(hash, out wt);

            return wt;
        }

        public void DebugTextures()
        {
            string debug = "";
            long totalSize = 0;

            foreach (KeyValuePair<string, CachedTexture> wt in textures)
            {
                debug += wt.Value.hash + wt.Value.width + "x" + wt.Value.height + " ref: " + wt.Value.refCount + " size: " + ToMB(wt.Value.memorySize) + "del: " + wt.Value.markForDeletion + "\n";
                totalSize += wt.Value.memorySize;
            }

            Debug.Log(debug);
            Debug.Log("Total Size: " + ToMB(totalSize));
            Debug.Log("GC Total Memory: " + System.GC.GetTotalMemory(false));
        }

        string ToMB(long bytes)
        {
            return ((float)bytes / (1024.0f * 1024.0f)).ToString("0.00") + "MB";
        }

        public void Update()
        {
            updateTimer += Time.deltaTime;

            if (updateTimer < runUpdatesAt) return;

            updateTimer = 0.0f;

            float currentTime = Time.fixedTime;

            var texturesEnum = textures.GetEnumerator();

            List<string> todelete = new List<string>();

            while (texturesEnum.MoveNext())
            {
                if (!texturesEnum.Current.Value.markForDeletion) continue;
                if (currentTime - texturesEnum.Current.Value.markedForDeletionTime < deleteAfterDelayOf) continue;
                todelete.Add(texturesEnum.Current.Value.hash);
            }

            for (var i = 0; i < todelete.Count; ++i) textures.Remove(todelete[i]);

        }

        public void Clear()
        {
            textures.Clear();
        }


    }
}

