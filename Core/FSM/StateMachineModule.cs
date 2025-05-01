using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Core.FSM {
    public class StateMachineModule : GameModule {
        public string OnStateEnterEventName => "FSM.OnStateEnter";
        public string OnStateExitEventName => "FSM.OnStateExit";
        private readonly Dictionary<string, StateMachine> m_StateMachines = new();

        private Queue<string> m_RemoveQueue = new();
        private bool m_IsActive = true;
        
        public override void OnUpdate() {
            if (!m_IsActive) return;

            while (m_RemoveQueue.Count > 0) {
                var fsmName = m_RemoveQueue.Dequeue();
                m_StateMachines.Remove(fsmName);
            }

            var keys = m_StateMachines.Keys.ToArray();
            foreach (var key in keys) {
                if (!m_StateMachines.ContainsKey(key)) {
                    Debug.LogWarning($"[FSM] Fsm: {key} does not exist");
                }
                else if (m_StateMachines[key] == null) {
                    Debug.LogWarning($"[FSM] Fsm: {key} is null");
                }
                else {
                    var stateMachine = m_StateMachines[key];
                    stateMachine.Update();
                }
            }
        }

        public Dictionary<string, StateMachine> GetData() {
            return m_StateMachines;
        }

        public string GetCurrentState(string fsmName) {
            if (m_StateMachines.ContainsKey(fsmName)) {
                return m_StateMachines[fsmName].GetCurrentState();
            }

            Debug.LogError("[FSM] Fsm: " + fsmName + " does not exist");
            return null;
        }

        public void SetInitialState(string fsmName, string stateName) {
            if (!IsFsmExisted(fsmName)) return;

            m_StateMachines[fsmName].SetInitialState(stateName);
        }

        public void ChangeState(string fsmName, string stateName) {
            if (!IsFsmExisted(fsmName)) return;

            m_StateMachines[fsmName].ChangeState(stateName);
        }

        public void AddState(string fsmName, string stateName, BaseState state) {
            if (!IsFsmExisted(fsmName)) return;

            m_StateMachines[fsmName].AddState(stateName, state);
        }

        public void AddActionState(string fsmName, string stateName, Action onEnter, Action onUpdate, Action onExit) {
            if (!IsFsmExisted(fsmName)) return;

            var state = new ActionState(m_StateMachines[fsmName], onEnter, onUpdate, onExit);

            m_StateMachines[fsmName].AddState(stateName, state);
        }

        public void RemoveState(string fsmName, string stateName) {
            if (!IsFsmExisted(fsmName)) return;

            m_StateMachines[fsmName].RemoveState(stateName);
        }

        public bool IsFsmExisted(string fsmName) {
            return m_StateMachines.ContainsKey(fsmName);
        }

        public StateMachine CreateStateMachine(string fsmName) {
            var fsm = StateMachine.Create(fsmName);
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
                m_RemoveQueue.Enqueue(fsmName);
            }
            else {
                Debug.LogWarningFormat("[FSM] Can not remove fsm with name: {0} because it does not exist", fsmName);
            }
        }

        public void StartStateMachine(string fsmName) {
            if (m_StateMachines.ContainsKey(fsmName)) {
                m_StateMachines[fsmName].Start();
            }
            else {
                Debug.LogWarningFormat("[FSM] Can not start fsm with name: {0} because it does not exist", fsmName);
            }
        }

        public void StopStateMachine(string fsmName) {
            if (m_StateMachines.ContainsKey(fsmName)) {
                m_StateMachines[fsmName].Stop();
            }
            else {
                Debug.LogWarningFormat("[FSM] Can not stop fsm with name: {0} because it does not exist", fsmName);
            }
        }

        /// <summary>
        /// stop fsm and remove all states for that fsm
        /// </summary>
        public void ClearStates(string fsmName) {
            if (!IsFsmExisted(fsmName)) {
                return;
            }

            StopStateMachine(fsmName);
            m_StateMachines[fsmName].ClearStates();
        }

        public void StopAllStateMachines() {
            m_IsActive = false;
        }

        public void StartAllStateMachines() {
            m_IsActive = true;
        }

        public override void OnReset() {
            m_StateMachines.Clear();
            m_RemoveQueue.Clear();
        }

        protected override void Awake() {
            base.Awake();
            Game.Fsm = this;
        }
    }
}