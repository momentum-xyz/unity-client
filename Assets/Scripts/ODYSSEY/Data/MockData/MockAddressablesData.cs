using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MockAddressable
{
    public string address;
    public string ID;
    public GameObject localPrefab;
}

[CreateAssetMenu]
public class MockAddressablesData : ScriptableObject
{
    public List<MockAddressable> mockAddressables;
}
