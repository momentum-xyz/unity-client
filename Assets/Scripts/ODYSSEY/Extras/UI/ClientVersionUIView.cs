using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ClientVersionUIView : MonoBehaviour
{

    public Text clientVersion;


    private void Awake()
    {
        clientVersion = GetComponent<Text>();

        if (clientVersion != null)
        {
            clientVersion.text = "v" + Globals.UNITYCLIENTVERSION;
        }
    }

}
