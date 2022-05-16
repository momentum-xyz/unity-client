using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HS;

public class AlphaStructureDriver : MonoBehaviour, IInfoUICapable
{
    public Guid guid
    {
        set
        {
            _guid = value;
            if (platformDriver != null)
            {
                platformDriver.guid = value;
            }
        }

        get
        {
            return _guid;
        }
    }

    private Guid _guid = Guid.Empty;
    public Transform parentTransform = null;
    public float lastVisit = 0f;                                            // last time the user wisp was in range of this structure
    public bool neverUnload = false;                                        // never unload textures (for program structures)
    public Vector3 teleportOffset = Vector3.zero;
    public List<HS.AutoRotator> autoRotators;
    public Dictionary<string, List<ITextureSlot>> textureSlots;
    public Dictionary<string, List<ITextSlot>> textSlots;
    public IStructureState structuresState;
    private IWorldBehaviour[] worldBehaviours;
    public int currentLOD = 0;                                              // what LOD is currently loaded?
    private UserPlatformDriver platformDriver = null;
    public bool LookAtParent = true;
    public Transform teleportPoint;
    public Transform customCenter; // used for objects with irregular pivots

    private void Awake()
    {
        FindAllStates();
        FindAllTextureSlots();
        FindAllTextSlots();
        FindAllWorldBehaviours();
        platformDriver = GetComponent<UserPlatformDriver>();
    }

    public void InitBehaviours()
    {
        if (worldBehaviours != null)
        {
            for (var i = 0; i < worldBehaviours.Length; ++i)
            {
                worldBehaviours[i].InitBehaviour();
            }
        }
    }

    void FindAllWorldBehaviours()
    {
        worldBehaviours = GetComponents<IWorldBehaviour>();
    }

    void FindAllStates()
    {
        structuresState = GetComponent<IStructureState>();
    }

    void FindAllTextureSlots()
    {
        textureSlots = new Dictionary<string, List<ITextureSlot>>();

        ITextureSlot[] slots = this.GetComponentsInChildren<ITextureSlot>(true);

        for (var i = 0; i < slots.Length; ++i)
        {
            string label = slots[i].GetLabel();

            List<ITextureSlot> list = null;
            textureSlots.TryGetValue(label, out list);

            if (list == null)
            {
                list = new List<ITextureSlot>();
                textureSlots[label] = list;
            }

            list.Add(slots[i]);
        }
    }

    void FindAllTextSlots()
    {
        textSlots = new Dictionary<string, List<ITextSlot>>();

        ITextSlot[] slots = this.GetComponentsInChildren<ITextSlot>(true);

        for (var i = 0; i < slots.Length; ++i)
        {
            string label = slots[i].GetLabel();

            List<ITextSlot> list = null;
            textSlots.TryGetValue(label, out list);

            if (list == null)
            {
                list = new List<ITextSlot>();
                textSlots[label] = list;
            }

            list.Add(slots[i]);
        }
    }



    public void FillTextureSlot(string label, Texture2D texture)
    {

        // Send textures to UserPlatformDriver as well
        // this is added for compatibility with HappyShip asset workflow
        platformDriver?.SetTexture(label, texture, 1.0f);


        List<ITextureSlot> list = null;

        textureSlots.TryGetValue(label, out list);

        if (list != null)
        {

            for (var i = 0; i < list.Count; ++i)
            {
                list[i].SetTexture(texture);
            }
        }

    }

    public void FillTextureSlot(string label, Texture2D texture, float ratio)
    {
        // Send textures to UserPlatformDriver as well
        // this is added for compatibility with HappyShip asset workflow
        platformDriver?.SetTexture(label, texture, ratio);

        List<ITextureSlot> list = null;

        textureSlots.TryGetValue(label, out list);

        if (list != null)
        {
            for (var i = 0; i < list.Count; ++i)
            {
                list[i].SetTexture(texture, ratio);
            }
        }

    }

    public void FillTextSlot(string label, string text)
    {
        List<ITextSlot> list = null;

        textSlots.TryGetValue(label, out list);

        if (list != null)
        {
            for (var i = 0; i < list.Count; ++i)
            {
                list[i].SetText(label, text);
            }
        }

    }



    public void SetPrivacy(bool isPrivate, bool currentUserCanEnter)
    {
        if (worldBehaviours == null) return;

        for (var i = 0; i < worldBehaviours.Length; ++i)
        {
            worldBehaviours[i].UpdatePrivacy(isPrivate, currentUserCanEnter);
        }
    }

    public void EnableAutoRotators()
    {
        if (autoRotators == null) return;

        foreach (HS.AutoRotator ar in autoRotators)
        {
            ar.enabled = true;
        }
    }

    public void DisableAutoRotators()
    {
        if (autoRotators == null) return;

        foreach (HS.AutoRotator ar in autoRotators)
        {
            ar.enabled = false;
        }
    }

    public void SetLOD(int lodLevel)
    {
        if (currentLOD == lodLevel) return;

        if (lodLevel == 0)
        {
            //EnableAutoRotators();
        }
        else
        {
            //DisableAutoRotators();
        }


        if (worldBehaviours == null) return;

        for (var i = 0; i < worldBehaviours.Length; ++i)
        {
            worldBehaviours[i].UpdateLOD(lodLevel);
        }

        currentLOD = lodLevel;
    }

    public void SetState<T>(string label, T value)
    {
        if (structuresState == null)
        {
            //Debug.Log("StructureState is null for " + GUID.ToString());
            return;
        }

        structuresState.SetState<T>(label, value);
    }



    public T GetState<T>(string label)
    {
        if (structuresState == null)
        {
            Debug.Log("StructureState is null for " + guid.ToString());
            return default(T);
        }

        return structuresState.GetState<T>(label);
    }

    public void UpdateBehaviours(float dt)
    {
        for (var i = 0; i < worldBehaviours.Length; ++i)
        {
            worldBehaviours[i].UpdateBehaviour(dt);
        }
    }
}
