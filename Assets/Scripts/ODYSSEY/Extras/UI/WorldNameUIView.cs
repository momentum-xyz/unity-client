using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey;
using UnityEngine.UI;
using Odyssey.Networking;

public class WorldNameUIView : MonoBehaviour, IRequiresContext
{
    IMomentumContext _c;

    public Text worldIDText;
    public void Init(IMomentumContext context)
    {
        _c = context;
    }

    void OnEnable()
    {
        _c.Get<IPosBus>().OnPosBusMessage += OnPosBusMessage;
    }

    void OnDisable()
    {
        _c.Get<IPosBus>().OnPosBusMessage -= OnPosBusMessage;
    }

    void OnPosBusMessage(IPosBusMessage msg)
    {
        switch (msg)
        {
            case PosBusSetWorldMsg m:
                worldIDText.text = m.worldID.ToString();
                break;
        }
    }
}
