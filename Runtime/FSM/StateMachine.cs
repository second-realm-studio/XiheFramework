using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static System.String;

namespace XiheFramework.Runtime.FSM {
    [Serializable]
    public class StateMachine {
        public string FsmName => m_FsmName;

        private string m_FsmName;
        private string m_CurrentState;
        private string m_InitialState;
        private string m_NextState;

        private Dictionary<string, BaseState> m_States = new();

        public static StateMachine Create(string fsmName, string initialState) {
            return new StateMachine { m_FsmName = fsmName, m_InitialState = initialState };
        }

        public static StateMachine Create(string fsmName) {
            return new StateMachine() { m_FsmName = fsmName };
        }

        public int GetStateCount() {
            return m_States.Count;
        }

        public void SetInitialState(string stateName) {
            m_InitialState = stateName;
        }

        public string GetInitialState() {
            return m_InitialState;
        }

        public string GetCurrentState() {
            return m_CurrentState;
        }

        public void AddState(BaseState state) {
            m_States[state.StateName] = state;
        }

        public void RemoveState(string stateName) {
            if (m_States.ContainsKey(stateName)) m_States.Remove(stateName);
        }

        public void ClearStates() {
            m_States.Clear();
        }

        public void OnStart() {
            if (m_InitialState == null || !m_States.ContainsKey(m_InitialState)) m_InitialState = m_States.Keys.First();

            m_CurrentState = m_InitialState;

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

        public void OnUpdate() {
            if (IsNullOrEmpty(m_CurrentState)) return;

            if (m_ExitToEnter) {
                m_States[m_CurrentState].OnEnterInternal();
                m_ExitToEnter = false;
                m_EnterToUpdate = true;
                m_NextFrameLock = false;
            }

            if (m_UpdateToExit) {
                m_States[m_CurrentState].OnExitInternal();
                m_CurrentState = m_NextState;
                m_UpdateToExit = false;
                m_ExitToEnter = true;
                m_EnterToUpdate = false;
            }

            if (m_EnterToUpdate && m_NextFrameLock) {
                if (m_States.ContainsKey(m_CurrentState))
                    m_States[m_CurrentState].OnUpdateInternal();
                else
                    Debug.Log("[FSM] state " + m_CurrentState + " does not exist");
            }

            m_NextFrameLock = true;
        }

        /// <summary>
        ///     shutdown fsm
        /// </summary>
        public void OnExit() {
            m_States[m_CurrentState].OnExitInternal();
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