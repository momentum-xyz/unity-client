using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Odyssey
{
    public class ShutdownState : IState
    {
        IMomentumContext _c;

        public ShutdownState(IMomentumContext context)
        {
            _c = context;
        }

        public void OnEnter()
        {
            Debug.Log("Unity is shutting down!");
            _c.Get<INetworkingService>().Dispose();
        }

        public void OnExit()
        {

        }

        public void Update()
        {

        }
    }
}

