using System;
using System.Collections.Generic;
using XiheFramework.Core.Utility.DataStructure;
using XiheFramework.Runtime;

namespace XiheFramework.Core.FSM {
    public abstract class BaseState {
        private readonly StateMachine m_ParentStateMachine;
        private readonly MultiDictionary<string, string> m_EventHandlerIds = new();

        protected BaseState(StateMachine parentStateMachine) {
            m_ParentStateMachine = parentStateMachine;
        }

        internal void OnEnterInternal() {
            OnEnterCallback();
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