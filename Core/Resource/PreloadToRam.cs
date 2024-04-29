using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace XiheFramework.Core.Resource {
    public class PreloadToRam : MonoBehaviour {
        private string m_LevelSceneAddress;
        private string m_LevelDataAddress;

        private void Start() {
            var nextLevelName = GameCore.Blackboard.GetData<string>(GameCore.Scene.CurrentLevelDataName);
            m_LevelSceneAddress = nextLevelName;
            m_LevelDataAddress = $"LevelData_{nextLevelName}";
            GameCore.Resource.LoadAssetAsync<LevelDataSetting>(m_LevelDataAddress, OnLevelDataLoaded);
        }

        private void OnLevelDataLoaded(LevelDataSetting obj) {
            StartCoroutine(LoadLabelAsync(obj.dataLabels));
        }

        IEnumerator LoadLabelAsync(IEnumerable<string> label) {
            var handle = GameCore.Resource.LoadAssetsAsyncCoroutine(label, OnProgress, OnLoaded);
            yield return handle;

            GameCore.Scene.LoadSceneAsync(m_LevelSceneAddress, LoadSceneMode.Single, true);
        }

        private void OnLoaded(IEnumerable<Object> obj) {
            GameCore.Blackboard.RemoveData("PreloadAddressableData.LoadingProgress");
        }

        private void OnProgress(float obj) {
            GameCore.Blackboard.SetData("PreloadAddressableData.LoadingProgress", obj);
        }
    }
}