using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey.Networking;
using PosBusAPI;

namespace Odyssey
{
    public class NotificationsController : StateController
    {
        public NotificationsController(IMomentumContext context) : base(context)
        {

        }

        public override void OnEnter()
        {
            _c.Get<IPosBus>().OnPosBusMessage += OnPosBusMessage;
        }

        public override void OnExit()
        {
            _c.Get<IPosBus>().OnPosBusMessage -= OnPosBusMessage;
        }

        void OnPosBusMessage(IPosBusMessage msg)
        {
            switch (msg)
            {
                case PosBusRelayToReactMsg m:
                    _c.Get<IUnityToReact>().RelayRelayMessage(m.Target, m.Message);
                        break;
                case PosBusSimpleNotificationMsg m:
                    if (m.Destination == Destination.Both || m.Destination == Destination.React)
                    {
                        _c.Get<IUnityToReact>().RelayNotificationSimple((int)m.Kind, m.Flag, m.Message);
                    }
                    break;

            }
        }

    }
}
