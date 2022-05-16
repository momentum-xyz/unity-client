using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Odyssey.Networking;

namespace Odyssey
{
    public class SpawnWorldState : IState
    {
        IMomentumContext _c;
        StateController[] _controllers;


        public SpawnWorldState(IMomentumContext context)
        {
            _c = context;
            _controllers = new StateController[] {
                new WorldObjectsMetadataController(context)
            };
        }

        public void OnEnter()
        {


            foreach (var controller in _controllers)
            {
                controller.OnEnter();
            }

            SpawnWorld().Forget();
        }

        async UniTask SpawnWorld()
        {
            await _c.Get<ISpawner>().SpawnWholeWorld();

            await _c.Get<IWorldPrefabHolder>().PreloadAssets();

            SessionStats.FlushSession(_c.Get<ISessionData>().UserID.ToString(), _c.Get<ISessionData>().SessionID, _c.Get<ISessionData>().WorldID);

            // Restoring PosBus message processing
            _c.Get<IPosBus>().ProcessMessageQueue = true;

            _c.Get<IStateMachine>().SwitchState(typeof(WorldTickingState));
        }

        void UnloadLoadingScene()
        {
            Scene loadingScene = SceneManager.GetSceneByName("Loading");

            if (loadingScene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(loadingScene);
            }
        }

        public void OnExit()
        {
            foreach (var controller in _controllers)
            {
                controller.OnExit();
            }

            UnloadLoadingScene();
        }

        public void Update()
        {
            foreach (var controller in _controllers)
            {
                controller.Update();
            }
        }
    }

}