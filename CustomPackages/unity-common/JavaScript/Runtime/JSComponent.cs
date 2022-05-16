using UnityEngine;

namespace Odyssey.MomentumCommon.JavaScript
{
    public class JSComponent : MonoBehaviour
    {
        public string esBehaviour = "";
        [SerializeField] [Multiline] protected string parameters;

        protected string classReference;

        public ESReferences references { get; private set; }
        public Collider otherCollider { get; private set; }

        protected void Start()
        {
            // References
            references = gameObject.GetComponent<ESReferences>();

            // Register the script
            JSAPI.component = this;
            classReference = JSRuntime.InitializeNewESBehaviour(parameters, esBehaviour);
        }

        private void Update()
        {
            otherCollider = null;
            JSAPI.component = this;
            JSRuntime.Execute(classReference + @".update();");
        }

        private void OnTriggerEnter(Collider other)
        {
            otherCollider = other;
            JSAPI.component = this;
            JSRuntime.Execute(classReference + @".onTriggerEnter();");
        }
    }
}
