using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XiheFramework.Core.LogicTime;
using XiheFramework.Core.Serialization;
using XiheFramework.Core.Serialization.Serializable;
using XiheFramework.Core.Utility.DataStructure;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Entity {
    public abstract class GameEntity : MonoBehaviour {
        [SerializeField, HideInInspector]
        public uint presetEntityId;

        [SerializeField, HideInInspector]
        public uint presetOwnerId;

        [SerializeField, HideInInspector]
        public bool useLogicTime = true;

        public abstract string GroupName { get; }
        public string EntityAddress { get; internal set; }
        public uint EntityId { get; internal set; }
        public uint OwnerId { get; internal set; }

        public float TimeScale { get; protected set; }
        protected float ScaledDeltaTime => Time.unscaledDeltaTime * TimeScale;

        private readonly MultiDictionary<string, string> m_EventHandlerIds = new();

        public virtual void OnInitCallback() { }
        public virtual void OnUpdateCallback() { }
        public virtual void OnFixedUpdateCallback() { }
        public virtual void OnLateUpdateCallback() { }
        public virtual void OnDestroyCallback() { }

        public virtual SerializableEntityData OnSerializedCallBack() {
            var data = new SerializableEntityData();
            data.entityAddress = EntityAddress;
            data.position = transform.position;
            data.rotation = transform.rotation.eulerAngles;
            return data;
        }

        public virtual void OnDeserializedCallBack(SerializableEntityData data) { }

        internal void OnInitCallbackInternal() {
            TimeScale = 1;
            SubscribeEvent(Game.LogicTime.onSetGlobalTimeScaleEventName, OnSetGlobalTimeScale);
            // SubscribeEvent(Game.Serialization.OnSaveEventName, OnSave);
            // SubscribeEvent(Game.Serialization.OnLoadEventName, OnLoad);

            OnInitCallback();
        }

        internal void OnUpdateCallbackInternal() => OnUpdateCallback();

        internal void OnFixedUpdateCallbackInternal() => OnFixedUpdateCallback();

        internal void OnLateUpdateCallbackInternal() => OnLateUpdateCallback();

        internal void OnDestroyCallbackInternal() {
            foreach (var eventName in m_EventHandlerIds.Keys) {
                UnsubscribeEvent(eventName);
            }

            m_EventHandlerIds.Clear();

            OnDestroyCallback();
        }

        public void DestroyEntity() {
            Game.Entity.DestroyEntity(EntityId);
        }

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

        private void OnSetGlobalTimeScale(object sender, object e) {
            if (useLogicTime) {
                var args = (OnSetGlobalTimeScaleEventArgs)e;
                TimeScale = args.newTimeScale;
            }
        }

        private void Start() {
            if (EntityId == 0) {
                Game.Entity.RegisterInstantiatedEntity(this, presetEntityId, false, presetOwnerId);
            }
        }

        private void OnDestroy() {
            Game.Entity.UnregisterEntity(EntityId);
        }
    }
}