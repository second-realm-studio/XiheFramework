using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Entity {
    public abstract class GameEntity : MonoBehaviour {
        public bool destroyWithOwner = false;

        public uint EntityId { get; internal set; }

        public abstract string EntityGroupName { get; }

        /// <summary>
        /// Indexing name for Addressable
        /// </summary>
        public string EntityName => gameObject.name;

        public string FullEntityAddress => $"{EntityGroupName}_{EntityName}";

        public uint OwnerId { get; internal set; }

        private Dictionary<string, string> m_EventHandlerIds = new Dictionary<string, string>();

        internal void OnInitCallbackInternal() {
            if (destroyWithOwner) {
                SubscribeEvent(Game.Entity.onEntityDestroyedEvtName, OnEntityDestroyed);
            }

            OnInitCallback();
        }

        internal void OnUpdateCallbackInternal() => OnUpdateCallback();

        internal void OnFixedUpdateCallbackInternal() => OnFixedUpdateCallback();

        internal void OnLateUpdateCallbackInternal() => OnLateUpdateCallback();

        internal void OnDestroyCallbackInternal() {
            foreach (var handlerId in m_EventHandlerIds.Keys) {
                UnsubscribeEvent(handlerId);
            }

            OnDestroyCallback();
        }

        public virtual void OnInitCallback() { }
        public virtual void OnUpdateCallback() { }
        public virtual void OnFixedUpdateCallback() { }
        public virtual void OnLateUpdateCallback() { }
        public virtual void OnDestroyCallback() { }

        public void DestroyEntity() {
            Game.Entity.DestroyEntity(EntityId);
        }

        protected void SubscribeEvent(string eventName, EventHandler<object> eventHandler) {
            if (!m_EventHandlerIds.ContainsKey(eventName)) {
                var handlerId = Game.Event.Subscribe(eventName, eventHandler);
                m_EventHandlerIds.Add(eventName, handlerId);
            }
            else {
                Debug.LogError(
                    $"[ENTITY] Multiple subscriptions to event: {eventName} is not allowed. Check your base classes. Subscribing Skipped.");
            }
        }

        protected void UnsubscribeEvent(string eventName) {
            if (m_EventHandlerIds.TryGetValue(eventName, out var handlerId)) {
                Game.Event.Unsubscribe(eventName, handlerId);
                m_EventHandlerIds.Remove(eventName);
                Debug.Log($"[ENTITY] Unsubscribed event: {eventName} with handlerId: {handlerId}");
            }
        }

        private void OnEntityDestroyed(object sender, object e) {
            if (sender is not uint ownerId) {
                return;
            }

            if (ownerId == OwnerId) {
                DestroyEntity();
            }
        }

        // private void Start() { }
        // private void Update() { }
        // private void FixedUpdate() { }
        // private void LateUpdate() { }
        // private void OnDestroy() { }
    }
}