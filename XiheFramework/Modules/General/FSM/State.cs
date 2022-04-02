using UnityEngine;

namespace XiheFramework {
    [System.Serializable]
    public abstract class State<T> : BaseState {
        protected T Owner;

        protected State(StateMachine parentStateMachine, T owner) : base(parentStateMachine) {
            Owner = owner;
        }
    }
}