using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey.Networking;

namespace Odyssey
{
    public class WorldObjectsMetadataController : StateController
    {
        private IWorldDataService _worldDataService;
        private IWorldObjectsStateManager _worldObjectsStateManager;
        public WorldObjectsMetadataController(IMomentumContext context) : base(context)
        {

        }

        public override void OnEnter()
        {
            _worldDataService = _c.Get<IWorldDataService>();
            _worldObjectsStateManager = _c.Get<IWorldObjectsStateManager>();

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

                case PosBusSetAttributesMsg m:
                    for (var i = 0; i < m.attributes.Length; ++i)
                    {
                        switch (m.attributes[i].label)
                        {
                            case "stagemode":
                                _worldObjectsStateManager.SetState<int>(m.spaceID.ToString(), "stagemode", (int)m.attributes[i].attribute);
                                break;
                            case "private":
                                int privacyValue = m.attributes[i].attribute;
                                _worldDataService.UpdatePermissionsForObject(m.spaceID, privacyValue);
                                break;
                            default:
                                _worldObjectsStateManager.SetState<int>(m.spaceID.ToString(), m.attributes[i].label, (int)m.attributes[i].attribute);
                                break;
                        }

                    }

                    break;
                case PosBusSetTexturesMsg m:
                    _worldDataService.UpdateObjectTextures(m.objectID, m.textures);
                    break;
                case PosBusSetStringsMsg m:
                    WorldObject wo = _c.Get<IWorldData>().Get(m.spaceID);
                    if (wo == null) return;
                    _worldDataService.UpdateObjectStrings(wo, m.strings);

                    AlphaStructureDriver structureDriver = wo.GetStructureDriver();

                    if (structureDriver == null) return;

                    for (var i = 0; i < m.strings.Length; ++i)
                    {
                        structureDriver.FillTextSlot(m.strings[i].label, m.strings[i].data);
                    }

                    break;
            }
        }



    }
}
