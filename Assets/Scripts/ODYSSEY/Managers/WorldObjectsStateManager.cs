using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Odyssey
{
    public interface IWorldObjectsStateManager
    {
        public void SetState<T>(string guid, string label, T value);
        public T GetState<T>(string guid, string label);
    }

    /// <summary>
    /// Manages the state updates of the objects in the world. Used by the React layer to set/poll state on certain action (ex. turn on/off StageMode)
    /// </summary>
    public class WorldObjectsStateManager : IRequiresContext, IWorldObjectsStateManager
    {
        IMomentumContext _c;

        public void Init(IMomentumContext context)
        {
            this._c = context;
        }


        public void SetState<T>(string guid, string label, T value)
        {
            WorldObject wo = _c.Get<IWorldData>().Get(new System.Guid(guid));

            if (wo == null)
            {
                Logging.Log("[WorldObjecsStateManager] Could not find object with guid: " + guid);
                return;
            }

            AlphaStructureDriver structureDriver = wo.GetStructureDriver();

            if (structureDriver == null)
            {
                Logging.Log("[WorldObjecsStateManager] No StructureDriver for " + guid);
                return;
            }

            structureDriver?.SetState<T>(label, value);

        }


        public T GetState<T>(string guid, string label)
        {
            WorldObject wo = _c.Get<IWorldData>().Get(new System.Guid(guid));

            if (wo == null)
            {
                Logging.Log("[WorldObjecsStateManager] Could not find object with guid: " + guid);
                return default(T);
            }

            AlphaStructureDriver structureDriver = wo.GetStructureDriver();

            if (structureDriver != null)
            {
                return structureDriver.GetState<T>(label);
            }

            return default(T);
        }

    }

}
