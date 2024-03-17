using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using XiheFramework.Core.Base;

namespace XiheFramework.Core.Scene {
    public class SceneModule : GameModule {
        public readonly string CurrentLevelDataName = "Game.CurrentLevel";
        public string menuLevelAddress;
        public bool loadMenuOnSetup;

        internal override void OnLateStart() {
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
        public void LoadSceneAsync(string sceneAddress, LoadSceneMode loadSceneMode, bool activateOnLoad) {
            Addressables.LoadSceneAsync(sceneAddress, loadSceneMode, activateOnLoad);

            // StartCoroutine(LoadAsyncCo(sceneAddress, loadSceneMode, activateOnLoad));
        }

        // private IEnumerator LoadAsyncCo(string sceneName, LoadSceneMode loadSceneMode, bool activateOnLoad) {
        //     // AsyncOperationHandle<SceneInstance> loadAO = 
        //     Addressables.LoadSceneAsync(sceneName, loadSceneMode, activateOnLoad);
        //     // while (!loadAO.IsDone) {
        //     //     // Game.Event.Invoke("Scene.OnLoadProgressing", sceneName, loadAO.PercentComplete);
        //     //     yield return null;
        //     // }
        //
        //     // Game.Event.Invoke("Scene.OnLoadFinished", sceneName, loadAO.PercentComplete);
        // }

        // public void UnloadSceneAsync(string sceneName) {
        //     StartCoroutine(UnLoadAsyncCo(sceneName));
        // }

        // private IEnumerator UnLoadAsyncCo(string sceneName) {
        //     AsyncOperationHandle<SceneInstance> unloadAO = Addressables.UnloadSceneAsync();
        //     while (!unloadAO.IsDone) {
        //         Game.Event.Invoke("Scene.OnUnloadProgressing", sceneName, unloadAO.PercentComplete);
        //         yield return null;
        //     }
        //
        //     Game.Event.Invoke("Scene.OnUnloadFinished", sceneName, unloadAO.PercentComplete);
        // }
    }
}