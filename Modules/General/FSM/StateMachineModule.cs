using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework {
    public class StateMachineModule : GameModule {
        private readonly Dictionary<string, StateMachine> m_StateMachines = new Dictionary<string, StateMachine>();

        private bool m_IsActive = true;

        public string GetCurrentState(string fsmName) {
            return m_StateMachines[fsmName].GetCurrentState();
        }

        public StateMachine CreateStateMachine(string fsmName) {
            var fsm = StateMachine.Create();
            if (m_StateMachines.ContainsKey(fsmName)) {
                m_StateMachines[fsmName] = fsm;
            }
            else {
                m_StateMachines.Add(fsmName, fsm);
            }

            return fsm;
        }

        public void PauseAllStateMachines() {
            m_IsActive = false;
        }

        public void ContinueAllStateMachines() {
            m_IsActive = true;
        }

        public override void Update() {
            if (!m_IsActive) return;

            foreach (var stateMachine in m_StateMachines.Values) {
                stateMachine.Update();
            }
        }

        public override void ShutDown(ShutDownType shutDownType) {
             m_StateMachines.Clear();
        }

    }
}