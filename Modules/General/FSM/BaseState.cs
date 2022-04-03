using System;

namespace XiheFramework {
    public abstract class BaseState {
        private StateMachine m_ParentStateMachine;
        
        protected BaseState(StateMachine parentStateMachine) {
            m_ParentStateMachine = parentStateMachine;
        }

        public abstract void OnEnter();
        
        public abstract void OnUpdate();
        
        public abstract void OnExit();

        protected void ChangeState(string targetState) {
            m_ParentStateMachine.ChangeState(targetState);
        }
    }
}