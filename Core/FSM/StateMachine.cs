using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XiheFramework.Entry;
using static System.String;

namespace XiheFramework.Core.FSM {
    [Serializable]
    public class StateMachine {
        private string m_CurrentState;

        private string m_DefaultState;

        private string m_NextState;
        private Dictionary<string, BaseState> m_States = new();

        public static StateMachine Create(string defaultState) {
            return new StateMachine {
                m_DefaultState = defaultState
            };
        }

        public static StateMachine Create() {
            return new StateMachine();
        }
        
        public int GetStateCount() {
            return m_States.Count;
        }

        public void SetDefaultState(string stateName) {
            m_DefaultState = stateName;
        }
        
        public string GetDefaultState() {
            return m_DefaultState;
        }

        public string GetCurrentState() {
            return m_CurrentState;
        }

        public void AddState(string stateName, BaseState state) {
            if (!m_States.ContainsKey(stateName))
                m_States.Add(stateName, state);
            else
                m_States[stateName] = state;
        }

        public void RemoveState(string stateName) {
            if (m_States.ContainsKey(stateName)) m_States.Remove(stateName);
        }

        public void ClearStates() {
            m_States.Clear();
        }

        public void Start() {
            if (m_DefaultState == null || !m_States.ContainsKey(m_DefaultState)) m_DefaultState = m_States.Keys.First();

            m_CurrentState = m_DefaultState;

            m_ExitToEnter = true;
            // m_States[m_CurrentState].OnEnter();
        }

        public void ChangeState(string targetState) {
            //when targetState has not been added to this state machine
            if (!m_States.ContainsKey(targetState)) {
                Debug.LogErrorFormat("[FSM] Current FSM does not contain the target state: {0}", targetState);
                return;
            }

            m_UpdateToExit = true;
            m_NextState = targetState;
            if (Game.Fsm.enableDebug) Debug.Log("Change to " + targetState);
        }

        public void Update() {
            if (IsNullOrEmpty(m_CurrentState)) return;

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
                if (m_States.ContainsKey(m_CurrentState))
                    m_States[m_CurrentState].OnUpdate();
                else
                    Debug.Log("[FSM] state " + m_CurrentState + " does not exist");
            }

            m_NextFrameLock = true;
        }

        /// <summary>
        ///     shutdown fsm
        /// </summary>
        public void Stop() {
            m_CurrentState = Empty;
        }

        #region Locks

        private bool m_ExitToEnter;
        private bool m_EnterToUpdate;
        private bool m_UpdateToExit;
        private bool m_NextFrameLock;

        #endregion
    }
}