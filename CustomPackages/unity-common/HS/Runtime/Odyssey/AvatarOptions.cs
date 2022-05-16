using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarOptions : MonoBehaviour
{
    public bool updateMaterial = false;
    public Material newMaterial;
    public bool updateSwitchToParticleDistance = false;
    public float newDistance = 10.0f;
    public GameObject FullWispPrefab;
}
