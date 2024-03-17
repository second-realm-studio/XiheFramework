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

        public override void OnEnter() {
            m_OnEnterCallbacks.Invoke();
        }

        public override void OnUpdate() {
            m_OnUpdateCallbacks.Invoke();
        }

        public override void OnExit() {
            m_OnExitCallbacks.Invoke();
        }
    }
}