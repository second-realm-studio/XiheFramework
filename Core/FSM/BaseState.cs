using System;
using System.Collections.Generic;
using XiheFramework.Core.Utility.DataStructure;
using XiheFramework.Runtime;

namespace XiheFramework.Core.FSM {
    public abstract class BaseState {
        public string StateName => m_StateName;
        private string m_StateName;
        private readonly StateMachine m_ParentStateMachine;
        private readonly MultiDictionary<string, string> m_EventHandlerIds = new();

        protected BaseState(StateMachine parentStateMachine, string stateName) {
            m_ParentStateMachine = parentStateMachine;
            m_StateName = stateName;
        }

        internal void OnEnterInternal() {
            OnEnterCallback();
            Game.Event.Invoke(Game.Fsm.OnStateEnterEventName, this, new OnStateEnteredEventArgs(m_ParentStateMachine.FsmName, m_StateName));
        }

        internal void OnUpdateInternal() {
            OnUpdateCallback();
        }

        internal void OnExitInternal() {
            foreach (var eventName in m_EventHandlerIds.Keys) {
                UnsubscribeEvent(eventName);
            }

            m_EventHandlerIds.Clear();
            OnExitCallback();
            Game.Event.Invoke(Game.Fsm.OnStateExitEventName, this, new OnStateExitedEventArgs(m_ParentStateMachine.FsmName, m_StateName));
        }

        protected abstract void OnEnterCallback();
        protected abstract void OnUpdateCallback();
        protected abstract void OnExitCallback();

        public void ChangeState(string targetState) {
            m_ParentStateMachine.ChangeState(targetState);
        }

        protected void SubscribeEvent(string eventName, EventHandler<object> eventHandler) {
            var handlerId = Game.Event.Subscribe(eventName, eventHandler);
            m_EventHandlerIds.Add(eventName, handlerId);
        }

        private void UnsubscribeEvent(string eventName) {
            if (m_EventHandlerIds.TryGetValue(eventName, out var handlerIds)) {
                foreach (var handlerId in handlerIds) {
                    Game.Event.Unsubscribe(eventName, handlerId);
                }
            }
        }
    }
}