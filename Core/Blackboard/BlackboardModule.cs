using System;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Blackboard {
    public class BlackboardModule : BlackboardModuleBase {
        public override int Priority => (int)CoreModulePriority.Blackboard;

        private readonly Dictionary<string, IBlackboard> m_Blackboards = new();

        #region Public Methods

        public override void CreateBlackboard<T>(string blackboardName) {
            var blackboard = new T();
            m_Blackboards[blackboardName] = blackboard;
        }

        public override T GetBlackboard<T>(string blackboardName) {
            if (!m_Blackboards.TryGetValue(blackboardName, out var blackboard)) {
                if (enableDebug) {
                    Debug.LogError($"[BLACKBOARD] Blackboard {blackboardName} not found");
                }

                return default;
            }

            return (T)blackboard;
        }

        public override IBlackboard GetBlackboard(string blackboardName) {
            if (!m_Blackboards.TryGetValue(blackboardName, out var blackboard)) {
                if (enableDebug) {
                    Debug.LogError($"[BLACKBOARD] Blackboard {blackboardName} not found");
                }

                return null;
            }

            return blackboard;
        }

        public override void ReleaseBlackboard(string blackboardName) {
            if (!m_Blackboards.TryGetValue(blackboardName, out var blackboard)) {
                if (enableDebug) {
                    Debug.LogError($"[BLACKBOARD] Blackboard {blackboardName} not found");
                }

                return;
            }

            blackboard.OnRelease();
            m_Blackboards.Remove(blackboardName);
        }

        #endregion
    }
}