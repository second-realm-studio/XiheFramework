﻿using System;
using UnityEngine;
using XiheFramework.Runtime.Utility.DataStructure;

namespace XiheFramework.Runtime.Base {
    public abstract class GameModuleBase : MonoBehaviour {
        /// <summary>
        /// Priority of the module to determine the order of execution
        /// The smaller the number, the earlier it will be executed
        /// Default value is 0
        /// </summary>
        public abstract int Priority { get; }

        public int updateInterval;
        public int fixedUpdateInterval;
        public int lateUpdateInterval;

        public bool enableDebug;

        #region Game Module Callbacks

        internal void OnInstantiatedInternal(Action onInstantiated) {
            OnInstantiated();
            onInstantiated?.Invoke();
        }

        internal void OnUpdateInternal() => OnUpdate();

        internal void OnFixedUpdateInternal() => OnFixedUpdate();

        internal void OnLateUpdateInternal() => OnLateUpdate();

        internal void OnDestroyedInternal() => OnDestroyed();

        /// <summary>
        /// Called after all modules that got instantiated at that frame are instantiated
        /// </summary>
        protected virtual void OnInstantiated() { }

        protected virtual void OnUpdate() { }
        protected virtual void OnFixedUpdate() { }
        protected virtual void OnLateUpdate() { }
        protected virtual void OnDestroyed() { }

        #endregion

        #region Event Management

        private readonly MultiDictionary<string, string> m_EventHandlerIds = new();

        /// <summary>
        /// Automatically Unsubscribe Event when Entity is being Destroyed
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventHandler"></param>
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

        #endregion
    }
}