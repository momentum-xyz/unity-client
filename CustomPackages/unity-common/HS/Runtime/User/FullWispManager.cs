using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FullWispManager : MonoBehaviour, IInfoUICapable
{
    public Guid guid { get; set; }
    public GameObject[] roleWispPrefabs;
    public GameObject[] roleWispSpawned;
    public string userID;
    public bool wispSwitched = false;
    public string wispName = "";
    public string wispOrganization = "";
    public bool poolLoaded = false;
    public Texture2D defaultBadge;
    public bool noCores = false;            // used to determine if prefab has multiple models inside it
    public GameObject noCoreWisp;
    public GameObject cachedNoCoreWisp;

    public void Start()
    {

        if (noCores == false)
        {
            roleWispSpawned = new GameObject[roleWispPrefabs.Length];

            for (int i = 0; i < roleWispPrefabs.Length; i++)
            {
                roleWispSpawned[i] = Instantiate(roleWispPrefabs[i], new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1));
                roleWispSpawned[i].transform.parent = this.gameObject.transform;
                roleWispSpawned[i].transform.localPosition = new Vector3(0, 0, 0);
                HS.AvatarDriver avatarDriver = roleWispSpawned[i].GetComponent<HS.AvatarDriver>();
                if (avatarDriver == null) continue;
                avatarDriver.SetBadge(defaultBadge);
                roleWispSpawned[i].SetActive(false);
            }

        }
        else
        {

            cachedNoCoreWisp = Instantiate(noCoreWisp, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1));
            cachedNoCoreWisp.transform.parent = this.gameObject.transform;
            cachedNoCoreWisp.transform.localPosition = new Vector3(0, 0, 0);
            HS.AvatarDriver avatarDriver = cachedNoCoreWisp.GetComponent<HS.AvatarDriver>();
            if (avatarDriver != null)
            {
                avatarDriver.SetBadge(defaultBadge);
            }

            cachedNoCoreWisp.SetActive(false);
        }

        this.gameObject.SetActive(false);

        poolLoaded = true;
    }

    public void SetRole(int roleNumber, string userID)
    {

        if (noCores == false)
        {

            // incase of invalid role
            if (roleNumber >= roleWispSpawned.Length)
            {
                roleNumber = 1;
            }

            for (int i = 0; i < roleWispSpawned.Length; i++)
            {
                roleWispSpawned[i].SetActive(false);
            }

            roleWispSpawned[roleNumber].SetActive(true);

            HS.AvatarDriver avatarDriver = roleWispSpawned[roleNumber].GetComponent<HS.AvatarDriver>();
            if (avatarDriver == null) return;
            avatarDriver.SetAvatarName(wispName);
            avatarDriver.SetAvatarOrganisation(wispOrganization);
            avatarDriver.SetBadge(defaultBadge);
            avatarDriver.name = "user " + userID + " " + wispName;
        }
        else
        {
            cachedNoCoreWisp.SetActive(true);

            HS.AvatarDriver avatarDriver = cachedNoCoreWisp.GetComponent<HS.AvatarDriver>();
            if (avatarDriver == null) return;
            avatarDriver.SetAvatarName(wispName);
            avatarDriver.SetBadge(defaultBadge);
            avatarDriver.name = "user " + userID + " " + wispName;
        }
    }

    public void SetName(string name)
    {
        this.wispName = name;

        HS.AvatarDriver avatarDriver = cachedNoCoreWisp.GetComponent<HS.AvatarDriver>();
        if (avatarDriver == null) return;
        avatarDriver.SetAvatarName(wispName);
    }

    public void OnDisable()
    {
        if (poolLoaded == true)
        {
            if (noCores == false)
            {
                for (int i = 0; i < roleWispPrefabs.Length; i++)
                {
                    roleWispSpawned[i].SetActive(false);
                }

                userID = "";
                wispName = "";
            }
            else
            {
                noCoreWisp.SetActive(false);

                userID = "";
                wispName = "";
            }
        }
    }

}
