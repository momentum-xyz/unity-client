using System.Collections;
using System.Collections.Generic;
using Odyssey;
using UnityEngine;

public class MomentumAPIController : StateController
{
    IMomentumContext _c;
    IMomentumAPI _api;

    public MomentumAPIController(IMomentumContext context) : base(context)
    {
        _c = context;
        _api = context.Get<IMomentumAPI>();
    }

    public override void OnEnter()
    {

    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        var nearBy = _c.Get<ILODSystem>().GetNearby();

        for (var i = 0; i < nearBy.Count; ++i)
        {
            if (nearBy[i].lodDirty)
            {
                _api.PublishLODUpdate(nearBy[i].guid, nearBy[i].LOD);
                _api.PublishTextureLODUpdate(nearBy[i].guid, nearBy[i].LOD, nearBy[i].texturesLOD);
                nearBy[i].lodDirty = false;
            }

            if (nearBy[i].texturesLODDirty)
            {
                _api.PublishTextureLODUpdate(nearBy[i].guid, nearBy[i].LOD, nearBy[i].texturesLOD);
                nearBy[i].texturesLODDirty = false;
            }

            _api.Update(nearBy[i].guid, Time.deltaTime);
        }
    }
}
