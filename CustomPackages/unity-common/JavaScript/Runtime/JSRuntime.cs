using UnityEngine;
using Jint;
using Jint.Runtime.Interop;
using System;

namespace Odyssey.MomentumCommon.JavaScript
{
    public static class JSRuntime
    {
        //
        //
        // Privates

        private static Engine engine;
        private static int nextScriptID = 0;

        //
        //
        // Constructor

        static JSRuntime()
        {
            engine = new Engine();

            engine.SetValue("log", new Action<object>(msg => Debug.LogWarning(msg)));
            engine.SetValue("API", TypeReference.CreateTypeReference(engine, typeof(JSAPI)));
            engine.SetValue("Vector2", TypeReference.CreateTypeReference(engine, typeof(Vector2)));
            engine.SetValue("Vector3", TypeReference.CreateTypeReference(engine, typeof(Vector3)));
            engine.SetValue("Quaternion", TypeReference.CreateTypeReference(engine, typeof(Quaternion)));
            engine.SetValue("Time", TypeReference.CreateTypeReference(engine, typeof(Time)));
            engine.SetValue("TimeUtil", TypeReference.CreateTypeReference(engine, typeof(TimeUtil)));
            engine.SetValue("Mathf", TypeReference.CreateTypeReference(engine, typeof(Mathf)));
            engine.SetValue("Random", TypeReference.CreateTypeReference(engine, typeof(UnityEngine.Random)));
        }

        //
        //
        // Methods

        public static void Execute(string source) => engine.Execute(source);

        public static string GetNewScriptID() => "script" + nextScriptID++;

        public static string InitializeNewESBehaviour(string parameters, string esBehaviour)
        {
            string scriptID = GetNewScriptID();

            engine.Execute(
                "const " + scriptID + " = new " + esBehaviour + "(" + parameters + ");"
            );

            return scriptID;
        }
    }
}
