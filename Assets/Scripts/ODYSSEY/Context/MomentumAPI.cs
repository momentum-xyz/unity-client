using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Odyssey;
using Cysharp.Threading.Tasks;

public class MomentumAPI : IMomentumAPI
{
    public Action OnUpdate { get; set; }

    private MomentumEvent<LodUpdateHandler> lodSubscribers = new MomentumEvent<LodUpdateHandler>();
    private MomentumEvent<UpdateHandler> updateSubscribers = new MomentumEvent<UpdateHandler>();
    private MomentumEvent<PrivacyUpdateHandler> privacySubscribers = new MomentumEvent<PrivacyUpdateHandler>();
    private MomentumEvent<IntDataUpdateHandler> intDataSubscribers = new MomentumEvent<IntDataUpdateHandler>();
    private MomentumEvent<StringDataUpdateHandler> stringDataSubscribers = new MomentumEvent<StringDataUpdateHandler>();
    private MomentumEvent<TextureUpdateHandler> textureDataSubscribers = new MomentumEvent<TextureUpdateHandler>();
    private MomentumEvent<EffectHandler> effectSubscribers = new MomentumEvent<EffectHandler>();
    private MomentumEvent<BridgeEffectHandler> bridgeEffectSubscribers = new MomentumEvent<BridgeEffectHandler>();
    private MomentumEvent<TextureLodHandler> textureLodSubscribers = new MomentumEvent<TextureLodHandler>();

    private IMomentumContext _c;

    private Texture2D defaultTexture = new Texture2D(0, 0);
    private Texture2D defaultMemeTexture;
    private Texture2D defaultPosterTexture;

    public MomentumAPI(IMomentumContext context)
    {
        _c = context;

        defaultTexture = Resources.Load<Texture2D>("Textures/textscreen_default");
        defaultMemeTexture = Resources.Load<Texture2D>("Textures/meme");
        defaultPosterTexture = Resources.Load<Texture2D>("Textures/poster");

    }

    // LOD

    public void RegisterForLODUpdates(IScriptable subscriber, LodUpdateHandler callback)
    {
        lodSubscribers.Register(subscriber, callback);

        WorldObject wo = _c.Get<IWorldData>().Get(subscriber.Owner);
        callback(wo.LOD);
    }

    public void UnregisterForLODUpdates(IScriptable subscriber)
    {
        lodSubscribers.Unregister(subscriber);
    }

    public void PublishLODUpdate(Guid guid, int lod)
    {
        if (!lodSubscribers.Subscribers.ContainsKey(guid))
        {
            return;
        }

        var subs = lodSubscribers.Subscribers[guid];
        for (var i = 0; i < subs.Count; ++i)
        {
            subs[i].Item2(lod);
        }
    }

    // Every Frame Updates

    public void RegisterForUpdates(IScriptable subscriber, UpdateHandler callback)
    {
        updateSubscribers.Register(subscriber, callback);
    }

    public void UnregisterForUpdates(IScriptable subscriber)
    {
        updateSubscribers.Unregister(subscriber);
    }

    public void Update(Guid guid, float dt)
    {
        if (!updateSubscribers.Subscribers.ContainsKey(guid)) return;

        var subs = updateSubscribers.Subscribers[guid];
        for (var i = 0; i < subs.Count; ++i)
        {
            subs[i].Item2(dt);
        }
    }

    // Privacy

    public void RegisterForPrivacyUpdates(IScriptable subscriber, PrivacyUpdateHandler callback)
    {
        privacySubscribers.Register(subscriber, callback);

        WorldObject wo = _c.Get<IWorldData>().Get(subscriber.Owner);
        callback(wo.privateMode > 0, wo.privateMode < 2);
    }

    public void UnregisterForPrivacyUpdates(IScriptable subscriber)
    {
        privacySubscribers.Unregister(subscriber);
    }

    public void PublishPrivacyUpdate(Guid guid, int privacy)
    {
        if (!privacySubscribers.Subscribers.ContainsKey(guid))
        {
            return;
        }

        var subs = privacySubscribers.Subscribers[guid];
        for (var i = 0; i < subs.Count; ++i)
        {
            subs[i].Item2(privacy > 0, privacy < 2);
        }
    }

    public void RegisterForIntDataUpdates(IScriptable subscriber, IntDataUpdateHandler callback)
    {
        intDataSubscribers.Register(subscriber, callback);

        WorldObject wo = _c.Get<IWorldData>().Get(subscriber.Owner);

        if (wo == null) return;

        foreach (var attr in wo.attributes)
        {
            callback(attr.Key, attr.Value);
        }
    }

    public void UnregisterForIntDataUpdates(IScriptable subscriber)
    {
        intDataSubscribers.Unregister(subscriber);
    }

    public void PublishIntData(Guid guid, string label, int value)
    {
        if (!intDataSubscribers.Subscribers.ContainsKey(guid))
        {
            return;
        }

        var subs = intDataSubscribers.Subscribers[guid];
        for (var i = 0; i < subs.Count; ++i)
        {
            subs[i].Item2(label, value);
        }
    }

    public void RegisterForStringDataUpdates(IScriptable subscriber, StringDataUpdateHandler callback)
    {
        stringDataSubscribers.Register(subscriber, callback);

        WorldObject wo = _c.Get<IWorldData>().Get(subscriber.Owner);

        if (wo == null) return;

        foreach (var tl in wo.textlabels)
        {
            callback(tl.Key, tl.Value);
        }
    }

    public void UnregisterForStringDataUpdates(IScriptable subscriber)
    {
        stringDataSubscribers.Unregister(subscriber);
    }

    public void PublishStringData(Guid guid, string label, string value)
    {
        if (!stringDataSubscribers.Subscribers.ContainsKey(guid))
        {
            return;
        }

        var subs = stringDataSubscribers.Subscribers[guid];
        for (var i = 0; i < subs.Count; ++i)
        {
            subs[i].Item2(label, value);
        }
    }

    public void RegisterForTextureUpdates(IScriptable subscriber, TextureUpdateHandler callback)
    {
        textureDataSubscribers.Register(subscriber, callback);

        callback("poster", defaultPosterTexture, 1.0f);
        callback("meme", defaultMemeTexture, 1.0f);
        callback("video", defaultTexture, 1.0f);
        callback("description", defaultTexture, 1.0f);
        callback("solution", defaultTexture, 1.0f);
        callback("problem", defaultTexture, 1.0f);
    }

    public void UnregisterForTextureUpdates(IScriptable subscriber)
    {
        textureDataSubscribers.Unregister(subscriber);
    }

    public void PublishTextureUpdate(Guid guid, string label, Texture2D texture, float ratio)
    {
        if (!textureDataSubscribers.Subscribers.ContainsKey(guid))
        {
            return;
        }

        var subs = textureDataSubscribers.Subscribers[guid];
        for (var i = 0; i < subs.Count; ++i)
        {
            subs[i].Item2(label, texture, ratio);
        }
    }

    public void RegisterForEffectUpdates(IScriptable subscriber, EffectHandler callback)
    {
        effectSubscribers.Register(subscriber, callback);
    }

    public void UnregisterForEffectUpdates(IScriptable subscriber)
    {
        effectSubscribers.Unregister(subscriber);
    }

    public void RegisterForBridgeEffectUpdates(IScriptable subscriber, BridgeEffectHandler callback)
    {
        bridgeEffectSubscribers.Register(subscriber, callback);
    }

    public void UnregisterForBridgeEffectUpdates(IScriptable subscriber)
    {
        bridgeEffectSubscribers.Unregister(subscriber);
    }

    public void PublishEffect(Guid guid, Vector3 position, GameObject go, int effectType)
    {
        if (!effectSubscribers.Subscribers.ContainsKey(guid))
        {
            return;
        }

        var subs = effectSubscribers.Subscribers[guid];
        for (var i = 0; i < subs.Count; ++i)
        {
            subs[i].Item2(position, go, effectType);
        }
    }

    public void PublishBridgeEffect(Guid guid, Vector3 sourcePosition, Vector3 destinationPosition, GameObject sourceGO, GameObject destinationGO, int effectType)
    {
        if (!bridgeEffectSubscribers.Subscribers.ContainsKey(guid))
        {
            return;
        }

        var subs = bridgeEffectSubscribers.Subscribers[guid];
        for (var i = 0; i < subs.Count; ++i)
        {
            subs[i].Item2(sourcePosition, destinationPosition, sourceGO, destinationGO, effectType);
        }
    }

    public void RegisterForTextureLODUpdates(IScriptable subscriber, TextureLodHandler callback)
    {
        textureLodSubscribers.Register(subscriber, callback);

        WorldObject wo = _c.Get<IWorldData>().Get(subscriber.Owner);

        if (wo == null) return;

        callback(wo.LOD, wo.texturesLOD);
    }

    public void UnregisterForTextureLODUpdates(IScriptable subscriber)
    {
        textureDataSubscribers.Unregister(subscriber);
    }

    public void PublishTextureLODUpdate(Guid guid, int objectLod, int lod)
    {
        if (!textureLodSubscribers.Subscribers.ContainsKey(guid))
        {
            return;
        }

        var subs = textureLodSubscribers.Subscribers[guid];
        for (var i = 0; i < subs.Count; ++i)
        {
            subs[i].Item2(objectLod, lod);
        }
    }
}

public class MomentumEvent<Handler>
{
    public Dictionary<Guid, List<(IScriptable, Handler)>> Subscribers => _subscribers;

    Dictionary<Guid, List<(IScriptable, Handler)>> _subscribers = new Dictionary<Guid, List<(IScriptable, Handler)>>();

    public void Register(IScriptable subscriber, Handler callback)
    {
        // Check if we add that guid for the first time
        if (!_subscribers.ContainsKey(subscriber.Owner))
        {
            _subscribers[subscriber.Owner] = new List<(IScriptable, Handler)>();
        }

        _subscribers[subscriber.Owner].Add((subscriber, callback));
    }

    public void Unregister(IScriptable subscriber)
    {
        if (!_subscribers.ContainsKey(subscriber.Owner)) return;

        List<(IScriptable, Handler)> subs = _subscribers[subscriber.Owner];

        for (var i = subs.Count - 1; i >= 0; i--)
        {
            if (subs[i].Item1 == subscriber)
            {
                subs.RemoveAt(i);
            }
        }
    }
}
