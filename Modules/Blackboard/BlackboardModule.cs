using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Modules.Base;
using XiheFramework.Utility;

namespace XiheFramework.Modules.Blackboard {
    public class BlackboardModule : GameModule {
        private readonly Dictionary<string, BlackBoardObject> m_Data = new();

        private TreeNode<string> m_DataPathTree;

        /// <summary>
        /// Clear all runtime data
        /// </summary>
        public void Reset() {
            m_Data.Clear();
            m_DataPathTree = null;
        }

        /// <summary>
        /// Retrive all data from the current blackboard
        /// </summary>
        /// <returns> data </returns>
        public object[] GetDataArray() {
            var result = new List<object>();
            foreach (var key in m_Data.Keys) result.Add(m_Data[key].entity);

            return result.ToArray();
        }

        /// <summary>
        /// Retrive all keys from the current blackboard
        /// </summary>
        /// <returns> keys </returns>
        public IEnumerable<string> GetDataPathArray() {
            return m_Data.Keys;
        }

        public TreeNode<string> GetDataPaths() {
            return m_DataPathTree;
        }


        /// <summary>
        /// Set data at runtime
        /// </summary>
        /// <param name="dataName">data name, work as indexing key</param>
        /// <param name="value">data entity</param>
        /// <param name="targetType">data type</param>
        /// <typeparam name="T">object</typeparam>
        public void SetData<T>(string dataName, T value, BlackBoardDataType targetType = BlackBoardDataType.Runtime) {
            if (m_Data.ContainsKey(dataName))
                m_Data[dataName] = new BlackBoardObject(value, targetType);
            else
                m_Data.Add(dataName, new BlackBoardObject(value, targetType));

            UpdateDataPathTree();
        }

        /// <summary>
        /// Get data entity at runtime
        /// </summary>
        /// <param name="dataName">data name, work as indexing key</param>
        /// <returns></returns>
        public object GetData(string dataName) {
            return GetData<object>(dataName);
        }

        /// <summary>
        /// Get data entity at runtime with a type conversion
        /// </summary>
        /// <param name="dataName">data name, work as indexing key</param>
        /// <typeparam name="T">output type</typeparam>
        /// <returns></returns>
        public T GetData<T>(string dataName) {
            if (m_Data.ContainsKey(dataName)) return (T)m_Data[dataName].entity;

            Debug.LogWarning("[BLACKBOARD] data does not exist. dataName: " + dataName);

            return default;
        }

        /// <summary>
        /// Check whether a data name exists
        /// </summary>
        /// <param name="dataName">data name, work as indexing key</param>
        /// <returns></returns>
        public bool ContainsKey(string dataName) {
            return m_Data.ContainsKey(dataName);
        }

        /// <summary>
        /// Remove a data entity from the runtime Blackboard using its data name
        /// </summary>
        /// <param name="dataName">data name, work as indexing key</param>
        public void RemoveData(string dataName) {
            if (m_Data.ContainsKey(dataName))
                m_Data.Remove(dataName);
            else
                Debug.LogWarningFormat("black board doesn't contain data name : " + dataName);
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
        }

        internal override void ShutDown(ShutDownType shutDownType) {
            Reset();
        }
    }
}