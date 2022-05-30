using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Odyssey.Utils
{
    public class MainThreadRunner : MonoBehaviour
    {
        public static MainThreadRunner Instance = null;
        private Action updateOnMainThread;

        private Queue<Action> runOnceOnMainThread = new Queue<Action>();
        private List<Action> tempListOfActions = new List<Action>();

        private void Awake()
        {
            if(Instance != null)
            {
                Logging.LogError("MainThreadRunner trying to run a second Instance..");
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
        }

        public static void RunOnce(Action a)
        {
            CheckForInstance();

            lock(Instance.runOnceOnMainThread)
            {
                Instance.runOnceOnMainThread.Enqueue(a);
            }

        }

        public static void AddToUpdate(Action a)
        {
            CheckForInstance();
            Instance.updateOnMainThread += a;
        }

        public static void RemoveFromUpdate(Action a)
        {
            if (Instance == null) return;

            Instance.updateOnMainThread -= a;
        }

        private static void CheckForInstance()
        {
            if (Instance != null) return;
            
            // if we don't have an Instance running, add a GameObject Container
            // and add MainThreadRunner to it
            GameObject go = new GameObject("Main Thread Runner");
            go.AddComponent<MainThreadRunner>();
            DontDestroyOnLoad(go);
        }

        void Update()
        {
            updateOnMainThread?.Invoke();

            // process runOnce queue
            // copy the queue to a new list and process them later
            // so we could unlock the runOnceMainThread queue faster
            tempListOfActions.Clear();
            lock(runOnceOnMainThread)
            {
                while(runOnceOnMainThread.Count != 0)
                {
                    tempListOfActions.Add(runOnceOnMainThread.Dequeue());
                }
            }

            for(var i=0; i < tempListOfActions.Count; ++i)
            {
                tempListOfActions[i].Invoke();
            }

        }

        private void OnDestroy()
        {
            updateOnMainThread = null;
        }
    }

}
