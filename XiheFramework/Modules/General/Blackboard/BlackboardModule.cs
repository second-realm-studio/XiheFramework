using System;
using System.Collections.Generic;

namespace XiheFramework {
    public class BlackboardModule : GameModule {
        private readonly Dictionary<string, object> m_RuntimeBlackboard = new Dictionary<string, object>();
        private readonly Dictionary<string, object> m_SaveDataBlackboard = new Dictionary<string, object>();
        private readonly Dictionary<string, object> m_GlobalSaveDataBlackboard = new Dictionary<string, object>();

        public BaseSaveData CreateSaveData() {
            BaseSaveData saveData = new BaseSaveData {
                saveData = m_SaveDataBlackboard,
            };
            return saveData;
        }

        public void LoadSaveData(BaseSaveData saveData) {
            m_SaveDataBlackboard.Clear();
            //给blackboard赋值
            foreach (var variable in saveData.saveData) {
                m_SaveDataBlackboard.Add(variable.Key, variable.Value);
            }

            //加载对应场景
            Game.Scene.LoadScene(Game.Blackboard.GetData<string>(XiheKeywords.VAR_CurrentSceneName));
        }


        public void SetData<T>(string dataName, T value, BlackBoardDataType targetType) {
            switch (targetType) {
                case BlackBoardDataType.Runtime:
                    if (m_RuntimeBlackboard.ContainsKey(dataName)) {
                        m_RuntimeBlackboard[dataName] = value;
                    }
                    else {
                        m_RuntimeBlackboard.Add(dataName, value);
                    }

                    if (m_SaveDataBlackboard.ContainsKey(dataName)) {
                        m_SaveDataBlackboard.Remove(dataName);
                    }

                    if (m_GlobalSaveDataBlackboard.ContainsKey(dataName)) {
                        m_GlobalSaveDataBlackboard.Remove(dataName);
                    }

                    break;
                case BlackBoardDataType.SaveData:
                    if (m_SaveDataBlackboard.ContainsKey(dataName)) {
                        m_SaveDataBlackboard[dataName] = value;
                    }
                    else {
                        m_SaveDataBlackboard.Add(dataName, value);
                    }

                    if (m_RuntimeBlackboard.ContainsKey(dataName)) {
                        m_RuntimeBlackboard.Remove(dataName);
                    }

                    if (m_GlobalSaveDataBlackboard.ContainsKey(dataName)) {
                        m_GlobalSaveDataBlackboard.Remove(dataName);
                    }

                    break;
                case BlackBoardDataType.GlobalSaveData:
                    if (m_GlobalSaveDataBlackboard.ContainsKey(dataName)) {
                        m_GlobalSaveDataBlackboard[dataName] = value;
                    }
                    else {
                        m_GlobalSaveDataBlackboard.Add(dataName, value);
                    }

                    if (m_SaveDataBlackboard.ContainsKey(dataName)) {
                        m_SaveDataBlackboard.Remove(dataName);
                    }

                    if (m_RuntimeBlackboard.ContainsKey(dataName)) {
                        m_RuntimeBlackboard.Remove(dataName);
                    }

                    break;
                default:
                    return;
                    throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null);
            }
        }

        public T GetData<T>(string dataName) {
            if (m_RuntimeBlackboard.ContainsKey(dataName)) {
                return (T) m_RuntimeBlackboard[dataName];
            }

            if (m_SaveDataBlackboard.ContainsKey(dataName)) {
                return (T) m_SaveDataBlackboard[dataName];
            }

            if (m_GlobalSaveDataBlackboard.ContainsKey(dataName)) {
                return (T) m_GlobalSaveDataBlackboard[dataName];
            }

            return default;
        }

        public bool ContainsKey(string dataName) {
            return m_RuntimeBlackboard.ContainsKey(dataName) ||
                   m_SaveDataBlackboard.ContainsKey(dataName) ||
                   m_GlobalSaveDataBlackboard.ContainsKey(dataName);
        }


        public void RemoveData<T>(string dataName) {
            if (m_RuntimeBlackboard.ContainsKey(dataName)) {
                m_RuntimeBlackboard.Remove(dataName);
            }
            else if (m_SaveDataBlackboard.ContainsKey(dataName)) {
                m_SaveDataBlackboard.Remove(dataName);
            }
            else if (m_GlobalSaveDataBlackboard.Remove(dataName)) {
                m_GlobalSaveDataBlackboard.Remove(dataName);
            }
            else {
                Game.Log.LogErrorFormat("black board doesn't contain data name : " + dataName);
            }
        }

        public void Reset() {
            m_RuntimeBlackboard.Clear();
            m_SaveDataBlackboard.Clear();
            m_GlobalSaveDataBlackboard.Clear();
        }

        public override void Update() {
        }

        public override void ShutDown(ShutDownType shutDownType) {
            Reset();
        }
    }
}