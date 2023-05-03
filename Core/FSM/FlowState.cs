using System;

namespace XiheFramework.Modules.FSM {
    public class FlowState : BaseState {
        private readonly Action onEnterCallbacks;
        private readonly Action onExitCallbacks;
        private readonly Action onUpdateCallbacks;

        public FlowState(StateMachine parentStateMachine, Action onEnterCallbacks, Action onUpdateCallbacks, Action onExitCallbacks) : base(
            parentStateMachine) {
            this.onEnterCallbacks = onEnterCallbacks;
            this.onUpdateCallbacks = onUpdateCallbacks;
            this.onExitCallbacks = onExitCallbacks;
        }

        public override void OnEnter() {
            onEnterCallbacks.Invoke();
        }

        public override void OnUpdate() {
            onUpdateCallbacks.Invoke();
        }

        public override void OnExit() {
            onExitCallbacks.Invoke();
        }
    }
}