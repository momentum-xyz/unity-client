using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey.Networking;

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
            _c.Get<IPosBus>().Disconnect();
        }

        public void OnExit()
        {

        }

        public void Update()
        {

        }
    }
}

