using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nextensions;
using Odyssey;
using UnityEngine.EventSystems;
using System;

public interface IUserInteraction
{
    public bool HasHoveredClickable { get; set; }
    public void HandleMouseClicks();
}

public class UserInteraction : IRequiresContext, IUserInteraction
{
    IMomentumContext _c;

    public void Init(IMomentumContext context)
    {
        this._c = context;
    }

    bool HasClickedUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public bool HasHoveredClickable
    {
        get
        {
            return HS.Clickable.HasSelection;
        }

        set { }
    }

    public void HandleMouseClicks()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!HS.Clickable.HasSelection || HasClickedUI()) return;

            if (HS.Clickable.SelectedIsAvatar)
            {
                FullWispManager fullWispManager = HS.Clickable.SelectedAvatar.GetComponentInParent<FullWispManager>();
                if (fullWispManager != null)
                    _c.Get<IUnityToReact>().SendProfileClickEvent(fullWispManager.userID, _c.Get<ISessionData>().WorldAvatarController.transform.position.ToString());

            }
            else if (HS.Clickable.SelectedIsPlatformElement)
            {
                string guid = HS.Clickable.Selection.Driver.guid.ToString();

                bool isStructurePrivate = _c.Get<IWorldDataService>().CanAccessObject(Guid.Parse(guid));
                bool processClick = false;

                processClick = !isStructurePrivate || (isStructurePrivate);

                if (!processClick) return;

                if (HS.Clickable.Selection is IClickable && HS.Clickable.Selection.GetLabel().Length > 0)
                {
                    _c.Get<IUnityToReact>().SendClick(guid, ((IClickable)HS.Clickable.Selection).GetLabel());
                }
                else
                {
                    Logging.Log("[UserInteraction] A clickable without set label " + guid);
                }

            }
            else
            {
                // Handle clicks on elements that are not part of the platform
                // TODO: Handle this on React side?
                if (HS.Clickable.Selection is IClickable && ((IClickable)HS.Clickable.Selection).GetLabel().Length > 0)
                {
                    Debug.Log("Clicked: " + HS.Clickable.Selection.GetLabel());
                }
            }
        }

    }
}
