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
#if USE_ADDRESSABLE
        /// <summary>
        /// Load scene async using Addressable
        /// </summary>
        /// <param name="sceneAddress"></param>
        /// <param name="loadSceneMode"></param>
        /// <param name="onSceneLoadComplete"></param>
        public void LoadSceneAsync(string sceneAddress, LoadSceneMode loadSceneMode, Action<AsyncOperationHandle<SceneInstance>> onSceneLoadComplete = null) {
            var handle = Addressables.LoadSceneAsync(sceneAddress, loadSceneMode);
            handle.Completed += operationHandle => {
                if (operationHandle.Status == AsyncOperationStatus.Succeeded) {
                    var activateHandle = operationHandle.Result.ActivateAsync();
                    activateHandle.completed += op => onSceneLoadComplete?.Invoke(operationHandle);
                }
            };
        }

        public void LoadScene(string sceneAddress, LoadSceneMode loadSceneMode, Action onSceneLoadComplete = null) {
            StartCoroutine(LoadSceneCo(sceneAddress, loadSceneMode, onSceneLoadComplete));
        }

        private IEnumerator LoadSceneCo(string address, LoadSceneMode loadSceneMode, Action onSceneLoadComplete) {
            var handle = Addressables.LoadSceneAsync(address, loadSceneMode);
            var sceneInstance = handle.WaitForCompletion();
            yield return sceneInstance.ActivateAsync();

            onSceneLoadComplete?.Invoke();
        }
#endif

        protected override void Awake() {
            base.Awake();

            Game.Scene = this;
        }
    }
}