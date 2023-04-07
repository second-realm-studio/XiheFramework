using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEngine;
using XiheFramework.Modules.Base;
using XiheFramework.Utility;

namespace XiheFramework.Modules.Event {
    public class EventModule : GameModule {
        private readonly MultiDictionary<string, EventHandler<object>> m_CurrentEvents = new();

        private readonly Queue<EventPair> m_WaitingList = new();

        private readonly object m_LockRoot = new();

        public override void Update() {
            lock (m_LockRoot) {
                while (m_WaitingList.Count > 0) {
                    var element = m_WaitingList.Dequeue();
                    element.EventHandler.Invoke(element.Sender, element.Argument);
                }
            }
        }

        public override void Setup() {
            base.Setup();
        }

        public override void ShutDown(ShutDownType shutDownType) {
            m_CurrentEvents.Clear();
        }

        public void Subscribe(string eventName, EventHandler<object> handler) {
            m_CurrentEvents.Add(eventName, handler);
        }

        public void Unsubscribe(string eventName, EventHandler<object> handler) {
            if (handler == null) {
                Debug.LogError("Handler is null");
                return;
            }

            m_CurrentEvents.Remove(eventName, handler);
        }

        public void Invoke(string eventName, object sender = null, object eventArg = null) {
            if (m_CurrentEvents.ContainsKey(eventName))
                foreach (var handler in m_CurrentEvents[eventName]) {
                    var eventPair = new EventPair(sender, eventArg, handler);
                    lock (m_LockRoot) {
                        m_WaitingList.Enqueue(eventPair);
                    }
                }
            else
                Debug.LogWarning("Event :" + eventName + " does not have any c# subscriber");

            Graph.SendGlobalEvent(eventName, eventArg, sender);
        }

        public int Count() {
            return m_CurrentEvents.Count;
        }

        private class EventPair {
            public readonly object Argument;
            public readonly EventHandler<object> EventHandler;
            public readonly object Sender;

            public EventPair(object sender, object argument, EventHandler<object> eventHandler) {
                Sender = sender;
                Argument = argument;
                EventHandler = eventHandler;
            }
        }
    }
}