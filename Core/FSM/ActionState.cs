using System;

namespace XiheFramework.Core.FSM {
    public class ActionState : BaseState {
        private readonly Action m_OnEnterCallbacks;
        private readonly Action m_OnExitCallbacks;
        private readonly Action m_OnUpdateCallbacks;

        public ActionState(StateMachine parentStateMachine, Action onEnterCallbacks, Action onUpdateCallbacks, Action onExitCallbacks) : base(
            parentStateMachine) {
            this.m_OnEnterCallbacks = onEnterCallbacks;
            this.m_OnUpdateCallbacks = onUpdateCallbacks;
            this.m_OnExitCallbacks = onExitCallbacks;
        }

        protected override void OnEnterCallback() {
            m_OnEnterCallbacks.Invoke();
        }

        protected override void OnUpdateCallback() {
            m_OnUpdateCallbacks.Invoke();
        }

        protected override void OnExitCallback() {
            m_OnExitCallbacks.Invoke();
        }
    }
}