namespace XiheFramework.Modules.FSM {
    public abstract class BaseState {
        private readonly StateMachine m_ParentStateMachine;

        protected BaseState(StateMachine parentStateMachine) {
            m_ParentStateMachine = parentStateMachine;
        }

        public abstract void OnEnter();

        public abstract void OnUpdate();

        public abstract void OnExit();

        public void ChangeState(string targetState) {
            m_ParentStateMachine.ChangeState(targetState);
        }
    }
}