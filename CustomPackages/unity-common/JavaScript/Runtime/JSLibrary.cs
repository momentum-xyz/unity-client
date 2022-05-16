using UnityEngine;
using System;

namespace Odyssey.MomentumCommon.JavaScript
{
    public class JSLibrary : MonoBehaviour
    {
        //
        //
        // Editor

        [SerializeField] protected TextAsset scriptFile;

        //
        //
        // Privates

        private String index;

        //
        //
        // Unity messages

        protected void Awake()
        {
            if (scriptFile == null)
            {
                Debug.LogError("Entry sript has not been assigned to " + gameObject.name);
                enabled = false;
                return;
            }

            index = scriptFile.ToString();

            JSRuntime.Execute(index);
        }
    }
}
