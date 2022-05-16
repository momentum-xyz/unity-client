using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Odyssey;
using Odyssey.Networking;

public class WorldEventsSimulator : MonoBehaviour, IRequiresContext
{

    IMomentumContext _c;

    public List<string> usersGuids;
    public string[] usersGuidsArray = null;

    public IMomentumContext ShaderContext()
    {
        return _c;
    }
    public void Init(IMomentumContext context)
    {
        this._c = context;

    }

    void OnEnable()
    {
        _c.Get<IPosBus>().OnPosBusMessage += OnPosBusMessage;

    }

    public void OnDisable()
    {
        _c.Get<IPosBus>().OnPosBusMessage -= OnPosBusMessage;
    }

    void OnPosBusMessage(IPosBusMessage msg)
    {
        switch (msg)
        {
            case PosBusPosMsg m:
                OnNewUser(m.userId, m.position);
                break;
            case PosBusRemoveUserMsg m:
                OnUserDelete(m.userId);
                break;
        }
    }

    void OnNewUser(Guid id, Vector3 pos)
    {
        if (id == _c.Get<ISessionData>().UserID) return;
        if (!usersGuids.Contains(id.ToString()))
        {
            usersGuids.Add(id.ToString());
            usersGuidsArray = usersGuids.ToArray();
        }

    }

    public void TeleportToUser(string userId)
    {
        _c.Get<IReactBridge>().TeleportToUser_Event?.Invoke(userId);
    }

    void OnUserDelete(Guid guid)
    {
        usersGuids.Remove(guid.ToString());
        usersGuidsArray = usersGuids.ToArray();
    }

    public void AddNewSpace(bool isRoot, string name, string spaceType)
    {
        _c.Get<IBackendService>().AddNewSpace(_c.Get<ISessionData>().WorldID.ToString(), isRoot, name, spaceType, _c.Get<ISessionData>().Token);
    }

    public void FlyToSpaceWithName(string name)
    {
        WorldObject worldObj = GetObjectByName(name);
        if (worldObj == null) return;
        _c.Get<IReactBridge>().TeleportToSpace_Event?.Invoke(worldObj.guid.ToString());
    }

    public WorldObject GetObjectByName(string name)
    {
        Dictionary<System.Guid, WorldObject> worldObjects = _c.Get<IWorldData>().WorldHierarchy;
        foreach (KeyValuePair<System.Guid, WorldObject> obj in worldObjects)
        {
            if (obj.Value.name == name)
            {
                return obj.Value;
            }
        }

        return null;
    }

    public void SimNewWorld(string worldID)
    {
        _c.Get<IPosBus>().TriggerTeleport(new Guid(worldID));
    }

    public void TriggerBridgeEffect(Guid emitter, Vector3 source, Vector3 destination, int type)
    {
        var msg = new PosBusTransitionalBridgingEffectOnPositionMsg(emitter, source, destination, (uint)type);
        _c.Get<IPosBus>().OnPosBusMessage(msg);
    }

    public void TriggerEffect(Guid emitter, Guid source, int type)
    {
        var msg = new PosBusTransitionalEffectOnObjectMsg(emitter, source, (uint)type);
        _c.Get<IPosBus>().OnPosBusMessage(msg);
    }

    public void TriggerInteractionMsg(uint kind, Guid targetId, int flag, string message)
    {
        _c.Get<IPosBus>().TriggerInteractionMsg(kind, targetId, flag, message);
    }

    public void UpdateRelayChainRadius(float newRadius)
    {
        HS.KUSA.GlobalResponder.RelayChainRadius = newRadius;
    }


}
