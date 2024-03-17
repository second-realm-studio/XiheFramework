using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XiheFramework.Entry;

namespace XiheFramework.Core.Resource {
    public class PreloadToRam : MonoBehaviour {
        private string m_LevelSceneAddress;
        private string m_LevelDataAddress;

        private void Start() {
            var nextLevelName = Game.Blackboard.GetData<string>(Game.Scene.CurrentLevelDataName);
            m_LevelSceneAddress = nextLevelName;
            m_LevelDataAddress = $"LevelData_{nextLevelName}";
            Game.Resource.LoadAssetAsync<LevelDataSetting>(m_LevelDataAddress, OnLevelDataLoaded);
        }

        private void OnLevelDataLoaded(LevelDataSetting obj) {
            StartCoroutine(LoadLabelAsync(obj.dataLabels));
        }

        IEnumerator LoadLabelAsync(IEnumerable<string> label) {
            var handle = Game.Resource.LoadAssetsAsyncCoroutine(label, OnProgress, OnLoaded);
            yield return handle;

            Game.Scene.LoadSceneAsync(m_LevelSceneAddress, LoadSceneMode.Single, true);
        }

        private void OnLoaded(IEnumerable<Object> obj) {
            Game.Blackboard.RemoveData("PreloadAddressableData.LoadingProgress");
        }

        private void OnProgress(float obj) {
            Game.Blackboard.SetData("PreloadAddressableData.LoadingProgress", obj);
        }
    }
}