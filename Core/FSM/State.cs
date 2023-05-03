using System;

namespace XiheFramework.Modules.FSM {
    [Serializable]
    public abstract class State<T> : BaseState {
        protected T owner;

        protected State(StateMachine parentStateMachine, T owner) : base(parentStateMachine) {
            this.owner = owner;
        }
    }
}