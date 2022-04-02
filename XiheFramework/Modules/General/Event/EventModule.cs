using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEngine;
using XiheFramework.Util;

namespace XiheFramework {
    public class EventModule : GameModule {
        private readonly MultiDictionary<string, EventHandler<object>> m_CurrentEvents =
            new MultiDictionary<string, EventHandler<object>>();

        private object m_LockRoot = new object();

        //private MultiDictionary<string, EventPair> m_WaitingList = new MultiDictionary<string, EventPair>();
        private readonly Queue<EventPair> m_WaitingList = new Queue<EventPair>();


        public override void Update() {
            // if (m_WaitingList.Count > 0)
            // {
            //     lock (m_LockRoot)
            //     {
            //         for (int i = 0; i < m_WaitingList.Count; i++)
            //         {
            //             var element = m_WaitingList.ElementAt(i);
            //
            //             for (int j = 0; j < element.Value.Count; j++)
            //             {
            //                 element.Value[j].EventHandler.Invoke(element.Value[j].Sender, element.Value[j].Argument);
            //             }
            //         }
            //     }
            // }
            // m_WaitingList.Clear();

            lock (m_LockRoot) {
                while (m_WaitingList.Count > 0) {
                    var element = m_WaitingList.Dequeue();
                    element.EventHandler.Invoke(element.Sender, element.Argument);
                }
            }
        }

        public override void ShutDown(ShutDownType shutDownType) {
            m_CurrentEvents.Clear();
        }

        public void Subscribe(string eventName, EventHandler<object> handler) {
            m_CurrentEvents.Add(eventName, handler);
        }

        public void Unsubscribe(string eventName, EventHandler<object> handler) {
            if (handler == null) {
                Game.Log.LogError("Handler is null");
                return;
            }

            m_CurrentEvents.Remove(eventName, handler);

            // if (m_CurrentEvents.ContainsKey(eventName)) {
            //     m_CurrentEvents[eventName] -= action;
            // }
        }

        public void Invoke(string eventName, object sender, object eventArg) {
            //lock (m_LockRoot)

            if (m_CurrentEvents.ContainsKey(eventName)) {
                foreach (var handler in m_CurrentEvents[eventName]) {
                    var eventPair = new EventPair(sender, eventArg, handler);
                    lock (m_LockRoot) {
                        m_WaitingList.Enqueue(eventPair);
                    }
                }
            }
            else {
                Debug.LogWarning("Event :" + eventName + " does not have any c# subscriber");
            }

            Graph.SendGlobalEvent(eventName, eventArg, sender);
        }

        /// <summary>
        /// 被注册，并且有回调函数的事件数量
        /// </summary>
        /// <returns></returns>
        public int Count() {
            return m_CurrentEvents.Count;
        }

        private class EventPair {
            public object Sender;
            public object Argument;
            public EventHandler<object> EventHandler;

            public EventPair(object sender, object argument, EventHandler<object> eventHandler) {
                Sender = sender;
                Argument = argument;
                EventHandler = eventHandler;
            }
        }
    }
}