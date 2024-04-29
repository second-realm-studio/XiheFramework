using System.Collections;
#if USE_ADDRESSABLE
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
#endif

using UnityEngine.SceneManagement;
using XiheFramework.Core.Base;

namespace XiheFramework.Core.Scene {
    public class SceneModule : GameModule {
        public readonly string CurrentLevelDataName = "Game.CurrentLevel";
        public string menuLevelAddress;
        public bool loadMenuOnSetup;

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
        public void LoadSceneAsync(string sceneAddress, LoadSceneMode loadSceneMode, bool activateOnLoad) {
#if USE_ADDRESSABLE
            Addressables.LoadSceneAsync(sceneAddress, loadSceneMode, activateOnLoad);
#endif
        }
    }
}