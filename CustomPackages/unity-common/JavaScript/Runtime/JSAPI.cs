using UnityEngine;

namespace Odyssey.MomentumCommon.JavaScript
{
    public static class JSAPI
    {
        public static JSComponent component;

        public static GameObject gameObject { get => component.gameObject; }
        public static Collider otherCollider { get => component.otherCollider; }
        public static ESReferences references { get => component.references; }

        public static void DestroyGameObject() => Object.Destroy(gameObject);
        public static void GetParticleSystemComponent() => gameObject.GetComponent<ParticleSystem>();
    }
}