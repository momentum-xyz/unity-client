using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Odyssey
{

    public interface IWorldData
    {

        public Dictionary<Guid, WorldObject> WorldHierarchy { get; }
        public Dictionary<Guid, List<IEffectsTrigger>> EffectsHandlers { get; }
        public List<Guid> WorldsList { get; }
        public List<GameObject> WorldDecorations { get; }
        public WorldObject Get(Guid key);
        public bool Exists(Guid key);
        public void Clear();
    }

    /// <summary>
    /// A global static class to hold all WorldData
    /// </summary>
    public class WorldData : IWorldData
    {
        public List<Guid> WorldsList
        {
            get
            {
                return worldsList;
            }

            internal set { }
        }

        public Dictionary<Guid, WorldObject> WorldHierarchy
        {
            get
            {
                return worldHierarchy;
            }

            internal set { }
        }

        public List<GameObject> WorldDecorations
        {
            get
            {
                return worldDecorations;
            }

            internal set { }
        }

        public Dictionary<Guid, List<IEffectsTrigger>> EffectsHandlers
        {
            get
            {
                return effectsHandlers;
            }

            internal set { }
        }

        public Dictionary<Guid, List<IEffectsTrigger>> effectsHandlers = new Dictionary<Guid, List<IEffectsTrigger>>();

        List<Guid> worldsList = new List<Guid>();
        Dictionary<Guid, WorldObject> worldHierarchy = new Dictionary<Guid, WorldObject>();
        List<GameObject> worldDecorations = new List<GameObject>();

        public WorldObject Get(Guid key)
        {
            if (worldHierarchy.ContainsKey(key))
            {
                return worldHierarchy[key];
            }

            return null;
        }

        public bool Exists(Guid key)
        {
            return worldHierarchy.ContainsKey(key);
        }

        public void Clear()
        {
            worldHierarchy.Clear();
            worldDecorations.Clear();
        }
    }

}