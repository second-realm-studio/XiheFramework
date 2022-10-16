using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using XiheFramework;

namespace XiheFramework {
    public class FlowState : BaseState {
        private Action onEnterCallbacks;
        private Action onUpdateCallbacks;
        private Action onExitCallbacks;

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