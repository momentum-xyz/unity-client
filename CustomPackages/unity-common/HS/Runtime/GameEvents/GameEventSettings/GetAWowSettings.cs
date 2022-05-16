using UnityEngine;


namespace HS
{
    [CreateAssetMenu(fileName = "Get A Wow Settings", menuName = "ScriptableObjects/Get a Wow Settings")]
    public class GetAWowSettings : ScriptableObject
    {
        public static GetAWowSettings Default;
        public bool IsDefault;

        public GameObject Prefab;
        
        void Awake()        { if( IsDefault ) Default = this;}
        void OnEnable()     { if( IsDefault ) Default = this;}
    }
}