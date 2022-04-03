using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace XiheFramework {
    public class SceneModule : GameModule {
        /// <summary>
        /// 异步加载
        /// </summary>
        private AsyncOperation m_AsyncOperation;

        private string m_CurrentSceneName;
        private int m_CurrentSceneId;

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="loadSceneName">场景名</param>
        public void LoadScene(string loadSceneName) {
            if (string.IsNullOrEmpty(loadSceneName)) {
                return;
            }

            m_AsyncOperation = SceneManager.LoadSceneAsync(loadSceneName);

            m_CurrentSceneName = loadSceneName;
            var scene = SceneManager.GetSceneByName(loadSceneName);
            m_CurrentSceneId = scene.buildIndex;

            Game.Blackboard.SetData(XiheKeywords.VAR_CurrentSceneName, m_CurrentSceneName, BlackBoardDataType.SaveData);
            Game.Event.Invoke(XiheKeywords.EVT_LoadSceneAsyncStart, this, null);
        }

        /// <summary>
        /// 返回场景序号
        /// </summary>
        /// <returns>场景序号</returns>
        public int GetSceneIndex() {
            return SceneManager.GetActiveScene().buildIndex;
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        private void StateUpdate() {
            //如果上一个场景还没有加载完，那就让他继续加载
            if (m_AsyncOperation != null) {
                if (m_AsyncOperation.progress >= 0.85) {
                    Game.Blackboard.SetData(XiheKeywords.VAR_LoadSceneAsyncProgress, 1f, BlackBoardDataType.Runtime);
                }
                else {
                    Game.Blackboard.SetData(XiheKeywords.VAR_LoadSceneAsyncProgress, m_AsyncOperation.progress, BlackBoardDataType.Runtime);
                }

                if (m_AsyncOperation.isDone) {
                    //Game.UI.UnActiveUI("LoadingSceneUI");
                    Game.Event.Invoke(XiheKeywords.EVT_LoadSceneAsyncEnd, this, null);
                    Game.Blackboard.SetData(XiheKeywords.VAR_CurrentSceneId, m_CurrentSceneId, BlackBoardDataType.Runtime);
                    Game.Blackboard.SetData(XiheKeywords.VAR_CurrentSceneName, m_CurrentSceneName, BlackBoardDataType.Runtime);
                }
            }
        }

        public override void Update() {
            StateUpdate();
        }

        public override void ShutDown(ShutDownType shutDownType) {
            m_CurrentSceneName = SceneManager.GetSceneByBuildIndex(0).name;
            m_CurrentSceneId = 0;
        }
    }
}