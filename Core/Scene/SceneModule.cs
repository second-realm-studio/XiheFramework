using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using XiheFramework;
using XiheFramework.Modules.Base;

public class SceneModule : GameModule {
    /// <summary>
    /// Load scene async using Addressable
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="loadSceneMode"></param>
    /// <param name="activateOnLoad"></param>
    public void LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode, bool activateOnLoad) {
        StartCoroutine(LoadAsyncCo(sceneName, loadSceneMode, activateOnLoad));
    }

    private IEnumerator LoadAsyncCo(string sceneName, LoadSceneMode loadSceneMode, bool activateOnLoad) {
        AsyncOperationHandle<SceneInstance> loadAO = Addressables.LoadSceneAsync(name, loadSceneMode, activateOnLoad);
        while (!loadAO.IsDone) {
            Game.Event.Invoke("Scene.OnLoadProgressing", sceneName, loadAO.PercentComplete);
            yield return null;
        }

        Game.Event.Invoke("Scene.OnLoadFinished", sceneName, loadAO.PercentComplete);
    }
}