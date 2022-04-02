using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static System.String;

namespace XiheFramework {
    [System.Serializable]
    public class StateMachine {
        private Dictionary<string, BaseState> m_States = new Dictionary<string, BaseState>();

        private string m_CurrentState;

        private string m_DefaultState;

        #region Locks

        private bool m_ExitToEnter = false;
        private bool m_EnterToUpdate = false;
        private bool m_UpdateToExit = false;
        private bool m_NextFrameLock = false;

        #endregion

        private string m_NextState;

        public static StateMachine Create(string fsmName) {
            return new StateMachine() {
                m_DefaultState = fsmName
            };
        }

        public static StateMachine Create() {
            return new StateMachine();
        }

        public void SetDefaultState(string stateName) {
            m_DefaultState = stateName;
        }

        public string GetCurrentState() {
            return m_CurrentState;
        }

        public void AddState(string stateName, BaseState state) {
            if (!m_States.ContainsKey(stateName)) {
                m_States.Add(stateName, state);
            }
            else {
                m_States[stateName] = state;
            }
        }

        public void RemoveState(string stateName) {
            if (m_States.ContainsKey(stateName)) {
                m_States.Remove(stateName);
            }
        }

        public void Start() {
            if (!m_States.ContainsKey(m_DefaultState)) {
                m_DefaultState = m_States.Keys.First();
            }

            m_CurrentState = m_DefaultState;

            m_ExitToEnter = true;
            // m_States[m_CurrentState].OnEnter();
        }

        public void ChangeState(string targetState) {
            m_UpdateToExit = true;
            m_NextState = targetState;
            // Debug.LogInfo("Change to "+targetState);
        }

        public void Update() {
            if (IsNullOrEmpty(m_CurrentState)) {
                return;
            }

            if (m_ExitToEnter) {
                m_States[m_CurrentState].OnEnter();
                m_ExitToEnter = false;
                m_EnterToUpdate = true;
                m_NextFrameLock = false;
            }

            if (m_UpdateToExit) {
                m_States[m_CurrentState].OnExit();
                m_CurrentState = m_NextState;
                m_UpdateToExit = false;
                m_ExitToEnter = true;
                m_EnterToUpdate = false;
            }

            if (m_EnterToUpdate && m_NextFrameLock) {
                if (m_States.ContainsKey(m_CurrentState)) {
                    m_States[m_CurrentState].OnUpdate();
                }
            }

            m_NextFrameLock = true;
        }

        public void ShutDown() {
            m_CurrentState = Empty;
        }
    }
}