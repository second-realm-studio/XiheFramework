using System;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Entity {
    //TODO: change to hash id
    public class EntityModule : GameModule {
        public readonly string onEntityInstantiatedEvtName = "Event.OnEntityRegistered";
        public readonly string onEntityDestroyedEvtName = "Event.OnEntityDestroyed";
        public readonly string onEntityOwnerChangedEvtName = "Event.OnEntityOwnerChanged";

        private readonly Dictionary<uint, GameEntity> m_Entities = new();

        private readonly object m_LockRoot = new();

        public T InstantiateEntity<T>(string entityAddress, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0, Action<T> onLoadCallback = null)
            where T : GameEntity {
            lock (m_LockRoot) {
                var entity = Game.Resource.InstantiateAsset<GameObject>(entityAddress).GetComponent<T>();
                if (entity == null) {
                    return null;
                }

                onLoadCallback?.Invoke(entity);
                SetUpGameEntity(entity, presetId, ownerEntityId, setParent);

                if (enableDebug) {
                    Debug.Log($"[ENTITY] Entity Instantiated : {entity.EntityId} ({entity.EntityName})");
                }

                return entity;
            }
        }

        public void InstantiateEntityAsync<T>(string entityAddress, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0, Action<T> onInstantiated = null)
            where T : GameEntity {
            Game.Resource.InstantiateAssetAsync<GameObject>(entityAddress, go => {
                var entity = go.GetComponent<T>();
                SetUpGameEntity(entity, presetId, ownerEntityId, setParent);
                onInstantiated?.Invoke(entity);
                if (enableDebug) {
                    Debug.Log($"[ENTITY] Entity Instantiated : {entity.EntityId} ({entity.EntityName})");
                }
            });
        }

        public void DestroyEntity(uint entityId) {
            lock (m_LockRoot) {
                if (m_Entities.ContainsKey(entityId)) {
                    var entity = m_Entities[entityId];
                    if (entity != null) {
                        entity.OnDestroyCallback();
                        if (enableDebug) {
                            Debug.Log($"[ENTITY] Entity Destroyed : {entityId} ({entity.EntityName})");
                        }

                        Destroy(entity.gameObject);
                    }

                    Game.Event.InvokeNow(onEntityDestroyedEvtName, entityId);
                    m_Entities.Remove(entityId);
                }
            }
        }

        public void ChangeEntityOwner(uint entityId, uint ownerId, bool setParent = true) {
            if (!m_Entities.ContainsKey(entityId)) {
                return;
            }

            if (!m_Entities.ContainsKey(ownerId)) {
                return;
            }

            m_Entities[entityId].OwnerId = ownerId;
            if (setParent) {
                m_Entities[entityId].transform.SetParent(m_Entities[ownerId].transform);
            }

            Game.Event.Invoke(onEntityOwnerChangedEvtName, entityId, ownerId);
        }

        public T GetEntity<T>(uint entityId) where T : GameEntity {
            if (m_Entities.ContainsKey(entityId)) {
                var result = m_Entities[entityId];
                return m_Entities[entityId] as T;
            }

            Debug.LogWarning($"[ENTITY] Entity : {entityId} is not Existed");
            return null;
        }

        public bool IsEntityExisted(uint entityId) {
            return m_Entities.ContainsKey(entityId);
        }

        public override void OnUpdate() {
            var cache = new List<uint>(m_Entities.Keys);
            foreach (var entity in cache) {
                if (m_Entities.ContainsKey(entity)) {
                    m_Entities[entity].OnUpdateCallbackInternal();
                }
            }
        }

        public override void OnFixedUpdate() {
            var cache = new List<uint>(m_Entities.Keys);
            foreach (var entity in cache) {
                if (m_Entities.ContainsKey(entity)) {
                    m_Entities[entity].OnFixedUpdateCallbackInternal();
                }
            }
        }

        public override void OnLateUpdate() {
            var cache = new List<uint>(m_Entities.Keys);
            foreach (var entity in cache) {
                if (m_Entities.ContainsKey(entity)) {
                    m_Entities[entity].OnLateUpdateCallbackInternal();
                }
            }
        }

        public override void OnReset() {
            var destroyQueue = new Queue<GameEntity>();
            foreach (var entity in m_Entities.Values) {
                destroyQueue.Enqueue(entity);
            }

            while (destroyQueue.Count > 0) {
                var entity = destroyQueue.Dequeue();
                entity.OnDestroyCallback();
                Destroy(entity.gameObject);
            }

            m_Entities.Clear();
        }

        private void SetUpGameEntity(GameEntity entity, uint presetId, uint ownerEntityId, bool setParent) {
            uint finalId = 1;
            if (presetId != 0) {
                if (m_Entities.ContainsKey(presetId)) {
                    m_Entities[presetId].OnDestroyCallbackInternal();
                    Destroy(m_Entities[presetId].gameObject);
                    m_Entities.Remove(presetId);
                    Game.Event.Invoke(onEntityDestroyedEvtName, presetId);
                }

                finalId = presetId;
            }
            else {
                while (m_Entities.ContainsKey(finalId)) {
                    finalId++;
                }
            }

            m_Entities.Add(finalId, entity);
            entity.EntityId = finalId;
            if (ownerEntityId != 0) {
                if (IsEntityExisted(ownerEntityId)) {
                    entity.OwnerId = ownerEntityId;
                    if (setParent) {
                        entity.transform.SetParent(GetEntity<GameEntity>(ownerEntityId).transform, false);
                    }
                }
                else {
                    Debug.LogWarning("[ENTITY] Entity OwnerId : " + ownerEntityId + $" is not existed. Entity {entity.EntityName} ownerId changed to 0(System Owner)");
                }
            }

            entity.OnInitCallbackInternal();
            Game.Event.Invoke(onEntityInstantiatedEvtName, finalId);
        }

        protected override void Awake() {
            base.Awake();

            Game.Entity = this;
        }
    }
}