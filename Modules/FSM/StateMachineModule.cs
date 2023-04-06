using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework {
    public class StateMachineModule : GameModule {
        public bool enableDebug;

        private readonly Dictionary<string, StateMachine> m_StateMachines = new Dictionary<string, StateMachine>();

        private bool m_IsActive = true;

        public Dictionary<string, StateMachine> GetData() {
            return m_StateMachines;
        }

        public string GetCurrentState(string fsmName) {
            if (m_StateMachines.ContainsKey(fsmName)) {
                return m_StateMachines[fsmName].GetCurrentState();
            }
            else {
                Debug.LogError("[FSM] Fsm: " + fsmName + " does not exist");
                return null;
            }
        }

        public void SetDefaultState(string fsmName, string stateName) {
            if (!IsFsmExisted(fsmName)) return;

            m_StateMachines[fsmName].SetDefaultState(stateName);
        }

        public void ChangeState(string fsmName, string stateName) {
            if (!IsFsmExisted(fsmName)) return;

            m_StateMachines[fsmName].ChangeState(stateName);
        }

        public void AddFlowState(string fsmName, string stateName, Action onEnter, Action onUpdate, Action onExit) {
            if (!IsFsmExisted(fsmName)) return;

            var state = new FlowState(m_StateMachines[fsmName], onEnter, onUpdate, onExit);

            m_StateMachines[fsmName].AddState(stateName, state);
        }

        public bool IsFsmExisted(string fsmName) {
            return m_StateMachines.ContainsKey(fsmName);
        }

        public StateMachine CreateStateMachine(string fsmName) {
            var fsm = StateMachine.Create();
            if (m_StateMachines.ContainsKey(fsmName)) {
                Debug.LogWarningFormat("[FSM] Fsm with name: {0} has already existed, replacing it..", fsmName);
                m_StateMachines[fsmName] = fsm;
            }
            else {
                m_StateMachines.Add(fsmName, fsm);
            }

            return fsm;
        }

        public void RemoveStateMachine(string fsmName) {
            if (m_StateMachines.ContainsKey(fsmName)) {
                m_StateMachines[fsmName].Stop();
                m_StateMachines.Remove(fsmName);
            }
            else {
                Debug.LogWarningFormat("[FSM] Can not remove fsm with name: {0} because it does not exist", fsmName);
            }
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