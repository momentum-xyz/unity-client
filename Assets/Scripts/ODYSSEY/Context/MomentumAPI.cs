using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Odyssey;

public class MomentumAPI : IMomentumAPI
{
    public Action OnUpdate { get; set; }

    private MomentumEvent<LodUpdateHandler> lodSubscribers = new MomentumEvent<LodUpdateHandler>();
    private MomentumEvent<UpdateHandler> updateSubscribers = new MomentumEvent<UpdateHandler>();
    private MomentumEvent<PrivacyUpdateHandler> privacySubscribers = new MomentumEvent<PrivacyUpdateHandler>();
    private MomentumEvent<IntDataUpdateHandler> intDataSubscribers = new MomentumEvent<IntDataUpdateHandler>();
    private MomentumEvent<StringDataUpdateHandler> stringDataSubscribers = new MomentumEvent<StringDataUpdateHandler>();

    private IMomentumContext _c;

    public MomentumAPI(IMomentumContext context)
    {
        _c = context;
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
