using System;
using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework {
    public class BlackboardModule : GameModule {
        // private readonly Dictionary<string, object> m_RuntimeBlackboard = new Dictionary<string, object>();
        // private readonly Dictionary<string, object> m_SaveDataBlackboard = new Dictionary<string, object>();
        // private readonly Dictionary<string, object> m_GlobalSaveDataBlackboard = new Dictionary<string, object>();

        private readonly Dictionary<string, BlackBoardObject> m_Data = new Dictionary<string, BlackBoardObject>();

        private TreeNode<string> m_DataPathTree;

        // public BaseSaveData CreateSaveData() {
        //     BaseSaveData saveData = new BaseSaveData {
        //         saveData = m_SaveDataBlackboard,
        //     };
        //     return saveData;
        // }
        //
        // public void LoadSaveData(BaseSaveData saveData) {
        //     m_SaveDataBlackboard.Clear();
        //     //给blackboard赋值
        //     foreach (var variable in saveData.saveData) {
        //         m_SaveDataBlackboard.Add(variable.Key, variable.Value);
        //     }
        //
        //     //加载对应场景
        //     //Game.Scene.LoadScene(Game.Blackboard.GetData<string>(XiheKeywords.VAR_CurrentSceneName));
        // }

        public object[] GetDataArray() {
            List<object> result = new List<object>();
            foreach (var key in m_Data.Keys) {
                result.Add(m_Data[key].entity);
            }

            return result.ToArray();
        }

        public IEnumerable<string> GetDataPathArray() {
            return m_Data.Keys;
        }

        public TreeNode<string> GetDataPaths() {
            return m_DataPathTree;
        }


        public void SetData<T>(string dataName, T value, BlackBoardDataType targetType = BlackBoardDataType.Runtime) {
            if (m_Data.ContainsKey(dataName)) {
                m_Data[dataName] = new BlackBoardObject(value, targetType);
            }
            else {
                m_Data.Add(dataName, new BlackBoardObject(value, targetType));
            }

            //Game.Event.Invoke("OnBlackBoardChanged", this, dataName);
            UpdateDataPathTree();

            // switch (targetType) {
            //     case BlackBoardDataType.Runtime:
            //         if (m_RuntimeBlackboard.ContainsKey(dataName)) {
            //             m_RuntimeBlackboard[dataName] = value;
            //         }
            //         else {
            //             m_RuntimeBlackboard.Add(dataName, value);
            //         }
            //
            //         if (m_SaveDataBlackboard.ContainsKey(dataName)) {
            //             m_SaveDataBlackboard.Remove(dataName);
            //         }
            //
            //         if (m_GlobalSaveDataBlackboard.ContainsKey(dataName)) {
            //             m_GlobalSaveDataBlackboard.Remove(dataName);
            //         }
            //
            //         break;
            //     case BlackBoardDataType.SaveData:
            //         if (m_SaveDataBlackboard.ContainsKey(dataName)) {
            //             m_SaveDataBlackboard[dataName] = value;
            //         }
            //         else {
            //             m_SaveDataBlackboard.Add(dataName, value);
            //         }
            //
            //         if (m_RuntimeBlackboard.ContainsKey(dataName)) {
            //             m_RuntimeBlackboard.Remove(dataName);
            //         }
            //
            //         if (m_GlobalSaveDataBlackboard.ContainsKey(dataName)) {
            //             m_GlobalSaveDataBlackboard.Remove(dataName);
            //         }
            //
            //         break;
            //     case BlackBoardDataType.GlobalSaveData:
            //         if (m_GlobalSaveDataBlackboard.ContainsKey(dataName)) {
            //             m_GlobalSaveDataBlackboard[dataName] = value;
            //         }
            //         else {
            //             m_GlobalSaveDataBlackboard.Add(dataName, value);
            //         }
            //
            //         if (m_SaveDataBlackboard.ContainsKey(dataName)) {
            //             m_SaveDataBlackboard.Remove(dataName);
            //         }
            //
            //         if (m_RuntimeBlackboard.ContainsKey(dataName)) {
            //             m_RuntimeBlackboard.Remove(dataName);
            //         }
            //
            //         break;
            //     default:
            //         return;
            //         throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null);
            // }
        }

        public object GetData(string dataName) {
            return GetData<object>(dataName);
        }

        public T GetData<T>(string dataName) {
            if (m_Data.ContainsKey(dataName)) {
                return (T) m_Data[dataName].entity;
            }

            // if (m_RuntimeBlackboard.ContainsKey(dataName)) {
            //     return (T) m_RuntimeBlackboard[dataName];
            // }
            //
            // if (m_SaveDataBlackboard.ContainsKey(dataName)) {
            //     return (T) m_SaveDataBlackboard[dataName];
            // }
            //
            // if (m_GlobalSaveDataBlackboard.ContainsKey(dataName)) {
            //     return (T) m_GlobalSaveDataBlackboard[dataName];
            // }

            return default;
        }

        public bool ContainsKey(string dataName) {
            // return m_RuntimeBlackboard.ContainsKey(dataName) ||
            //        m_SaveDataBlackboard.ContainsKey(dataName) ||
            //        m_GlobalSaveDataBlackboard.ContainsKey(dataName);
            return m_Data.ContainsKey(dataName);
        }

        public void RemoveData<T>(string dataName) {
            // if (m_RuntimeBlackboard.ContainsKey(dataName)) {
            //     m_RuntimeBlackboard.Remove(dataName);
            // }
            // else if (m_SaveDataBlackboard.ContainsKey(dataName)) {
            //     m_SaveDataBlackboard.Remove(dataName);
            // }
            // else if (m_GlobalSaveDataBlackboard.Remove(dataName)) {
            //     m_GlobalSaveDataBlackboard.Remove(dataName);
            // }
            if (m_Data.ContainsKey(dataName)) {
                m_Data.Remove(dataName);
            }
            else {
                Game.Log.LogErrorFormat("black board doesn't contain data name : " + dataName);
            }
        }

        private void UpdateDataPathTree() {
            m_DataPathTree = new TreeNode<string>("root");
            foreach (var key in m_Data.Keys) {
                var layers = key.Split('.');
                var currentLayer = m_DataPathTree;
                foreach (var layer in layers) {
                    var node = currentLayer.AddChild(layer);
                    currentLayer = node;
                }
            }

            //Debug.Log(m_DataPathTree.Flatten());
        }

        public void Reset() {
            m_Data.Clear();
            m_DataPathTree = null;
        }

        public override void Update() {
        }

        public override void ShutDown(ShutDownType shutDownType) {
            Reset();
        }
    }
}