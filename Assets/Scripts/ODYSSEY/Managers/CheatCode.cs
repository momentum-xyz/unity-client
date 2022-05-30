using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using Odyssey.Networking;
using UnityEngine.Networking;
using Odyssey;
using Cysharp.Threading.Tasks;
using TMPro;

public class CheatCode : MonoBehaviour, IRequiresContext
{
    private string[] cheatCode;
    private int index;
    private bool cheatMode = false;
    public GameObject cheatWindow;
    public GameObject cineWindow;
    public GameObject teleportWindow;
    public GameObject versionNumber;
    public GameObject position;
    private bool disabledText = false;
    public Text cheatWindowText;
    public InputField userUID;
    public InputField worldUID;
    public GameObject UIDs;
    public Button teleportButtonBase;

    public AlphaCineMode alphaCineMode;

    private bool worldButtonsCreated = false;

    internal IMomentumContext _c;

    void Start()
    {
        cheatCode = new string[] { "z", "q", "a", "z", "q", "a", "r", "t", "y" };
        index = 0;

        cheatMode = IsOnDevEnviorment();

    }

    public void Init(IMomentumContext context)
    {
        this._c = context;
    }

    public void Dispose()
    {

    }

    void teleportClick(Guid worldGuid)
    {
        _c.Get<IPosBus>().TriggerTeleport(worldGuid);
    }

    bool IsOnDevEnviorment()
    {
        if (Application.isEditor) return true;

        return false;
    }

    void Update()
    {

        if (_c == null) return;

        if (!cheatMode)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(cheatCode[index]))
                {
                    index++;
                }
                else
                {
                    index = 0;
                }
            }

            if (index == cheatCode.Length)
            {
                cheatMode = true;
                cheatWindow.SetActive(true);
                index = 0;
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (disabledText == false)
            {
                disabledText = true;
                versionNumber.SetActive(false);
                position.SetActive(false);
            }
            else
            {
                disabledText = false;
                versionNumber.SetActive(true);
                position.SetActive(true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            if (_c.Get<ISessionData>().WorldAvatarController != null)
            {
                var controller = _c.Get<ISessionData>().WorldAvatarController.GetComponent<HS.ThirdPersonController>();
                controller.CinematicMode = !controller.CinematicMode;
                Logging.Log("[CheatCode] Switched Cinematicmode to: " + controller.CinematicMode);
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (cheatWindow.activeInHierarchy == true)
            {
                cheatWindow.SetActive(false);
            }
            else
            {
                cheatWindow.SetActive(true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            if (cineWindow.activeInHierarchy == true)
            {
                cineWindow.SetActive(false);
                alphaCineMode.cineModeActive = false;
            }
            else
            {
                if (cheatWindow.activeInHierarchy == true)
                {
                    cheatWindow.SetActive(false);
                }
                cineWindow.SetActive(true);
                alphaCineMode.cineModeActive = true;
            }
        }
    }

    async UniTask populateWorldList()
    {

        var worlds = await _c.Get<IBackendService>().GetWorldsList();

        if (worlds == null) return;

        for (int i = 0; i < worlds.data.Length; i++)
        {
            _c.Get<IWorldData>().WorldsList.Add(Guid.Parse(worlds.data[i]));
        }
    }
}
