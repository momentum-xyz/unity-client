using UnityEngine;

namespace HS
{
    [CreateAssetMenu(fileName = "Arc Travel Settings", menuName = "ScriptableObjects/Arc Travel Settings", order = 1)]
    public class ArcTravelSettings : ScriptableObject
    {
        public static ArcTravelSettings Default;

        public AnimationCurve Curve;
        public float YFactor = 0.5f;
        public GameObject StartEffectPrefab;
        public GameObject EndEffectPrefab;
        public float Speed = 10;
        public float MaxTime = 3;


        public bool IsDefault;

        void Awake()        { if( IsDefault ) Default = this;}
        void OnEnable()     { if( IsDefault ) Default = this;}
        
    }
}