using System;
using System.Collections;
#if USE_ADDRESSABLE
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
#endif

using UnityEngine.SceneManagement;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Scene {
    public class SceneModule : GameModule {
        public readonly string currentLevelDataName = "Game.CurrentLevel";
        public string menuLevelAddress;
        public bool loadMenuOnSetup;

#if USE_ADDRESSABLE
        public override void OnLateStart() {
            if (loadMenuOnSetup) {
                LoadSceneAsync(menuLevelAddress, LoadSceneMode.Single, true);
            }
        }

        /// <summary>
        /// Load scene async using Addressable
        /// </summary>
        /// <param name="sceneAddress"></param>
        /// <param name="loadSceneMode"></param>
        /// <param name="activateOnLoad"></param>
        /// <param name="onSceneLoadComplete"></param>
        public void LoadSceneAsync(string sceneAddress, LoadSceneMode loadSceneMode, bool activateOnLoad, Action<AsyncOperationHandle<SceneInstance>> onSceneLoadComplete = null) {
            var handle = Addressables.LoadSceneAsync(sceneAddress, loadSceneMode, activateOnLoad);
            handle.Completed += onSceneLoadComplete;
        }

        public void LoadScene(string sceneAddress, LoadSceneMode loadSceneMode, bool activateOnLoad) {
            var handle = Addressables.LoadSceneAsync(sceneAddress, loadSceneMode, activateOnLoad);
            handle.WaitForCompletion();
        }
#endif

        protected override void Awake() {
            base.Awake();

            Game.Scene = this;
        }
    }
}