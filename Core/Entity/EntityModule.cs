using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Entity {
    //TODO: change to hash id
    public class EntityModule : GameModule, IEntityModule {
        public string OnEntityInstantiatedEvtName => "Event.OnEntityRegistered";
        public string OnEntityDestroyedEvtName => "Event.OnEntityDestroyed";
        public string OnEntityOwnerChangedEvtName => "Event.OnEntityOwnerChanged";

        private readonly Dictionary<uint, GameEntity> m_Entities = new();
        private readonly Dictionary<uint, uint> m_RecycledEntityIds = new(); //entity ids that's being destroyed at current frame

        private readonly object m_LockRoot = new();
        public GameEntity[] CurrentEntities => m_Entities.Values.ToArray();

        public void RegisterInstantiatedEntity<T>(T entity, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0)
            where T : GameEntity {
            if (entity.EntityId != 0) {
                Debug.LogWarning("[ENTITY] Entity : " + entity.EntityId + " has already been registered, no need to register again");
                return;
            }

            lock (m_LockRoot) SetUpGameEntity(entity, entity.gameObject.name, presetId, ownerEntityId, setParent);
        }

        public T InstantiateEntity<T>(string entityAddress, Vector3 localPosition, Quaternion localRotation, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0,
            Action<T> onInstantiatedCallback = null) where T : GameEntity {
            var go = Game.Resource.InstantiateAsset<GameObject>(entityAddress, localPosition, localRotation);
            if (go == null) {
                Debug.LogError("[ENTITY] Entity Instantiation Failed : " + entityAddress + " is not a GameObject");
                return null;
            }

            var entity = go.GetComponent<T>();
            if (entity == null) {
                Debug.LogError("[ENTITY] Entity Instantiation Failed : " + entityAddress + " has no component of type " + typeof(T));
                return null;
            }

            lock (m_LockRoot) SetUpGameEntity(entity, entityAddress, presetId, ownerEntityId, setParent, onInstantiatedCallback);

            if (enableDebug) {
                Debug.Log($"[ENTITY] Entity Instantiated : {entity.EntityId} ({entity.EntityAddress})");
            }

            return entity;
        }

        public T InstantiateEntity<T>(string entityAddress, Vector3 localPosition, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0,
            Action<T> onInstantiatedCallback = null)
            where T : GameEntity {
            return InstantiateEntity(entityAddress, localPosition, Quaternion.identity, ownerEntityId, setParent, presetId, onInstantiatedCallback);
        }

        public T InstantiateEntity<T>(string entityAddress, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0, Action<T> onInstantiatedCallback = null)
            where T : GameEntity {
            return InstantiateEntity(entityAddress, Vector3.zero, Quaternion.identity, ownerEntityId, setParent, presetId, onInstantiatedCallback);
        }

        public GameEntity InstantiateEntity(string entityAddress, Vector3 localPosition, Quaternion localRotation, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0,
            Action<GameEntity> onInstantiatedCallback = null) {
            return InstantiateEntity<GameEntity>(entityAddress, localPosition, localRotation, ownerEntityId, setParent, presetId, onInstantiatedCallback);
        }

        public GameEntity InstantiateEntity(string entityAddress, Vector3 localPosition, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0,
            Action<GameEntity> onInstantiatedCallback = null) {
            return InstantiateEntity<GameEntity>(entityAddress, localPosition, Quaternion.identity, ownerEntityId, setParent, presetId, onInstantiatedCallback);
        }

        public GameEntity InstantiateEntity(string entityAddress, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0,
            Action<GameEntity> onInstantiatedCallback = null) {
            return InstantiateEntity<GameEntity>(entityAddress, Vector3.zero, Quaternion.identity, ownerEntityId, setParent, presetId, onInstantiatedCallback);
        }

        public void InstantiateEntityAsync<T>(string entityAddress, Vector3 localPosition, Quaternion localRotation, uint ownerEntityId = 0, bool setParent = true,
            uint presetId = 0, Action<T> onInstantiatedCallback = null)
            where T : GameEntity {
            if (string.IsNullOrEmpty(entityAddress)) {
                Debug.LogError("[ENTITY] Entity address cannot be null or empty");
                return;
            }

            Game.Resource.InstantiateAssetAsync<GameObject>(entityAddress, localPosition, localRotation, go => {
                var entity = go.GetComponent<T>();
                lock (m_LockRoot) SetUpGameEntity(entity, entityAddress, presetId, ownerEntityId, setParent, onInstantiatedCallback);
                if (enableDebug) {
                    Debug.Log($"[ENTITY] Entity Instantiated : {entity.EntityId} ({entity.EntityAddress})");
                }
            });
        }

        public void InstantiateEntityAsync<T>(string entityAddress, Vector3 localPosition, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0,
            Action<T> onInstantiatedCallback = null) where T : GameEntity {
            InstantiateEntityAsync(entityAddress, localPosition, Quaternion.identity, ownerEntityId, setParent, presetId, onInstantiatedCallback);
        }

        public void InstantiateEntityAsync<T>(string entityAddress, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0, Action<T> onInstantiatedCallback = null)
            where T : GameEntity {
            InstantiateEntityAsync(entityAddress, Vector3.zero, Quaternion.identity, ownerEntityId, setParent, presetId, onInstantiatedCallback);
        }

        public void DestroyEntity(uint entityId) {
            lock (m_LockRoot) {
                if (!m_Entities.ContainsKey(entityId) || m_RecycledEntityIds.ContainsKey(entityId)) return;
                var entity = m_Entities[entityId];
                if (entity == null) {
                    Debug.LogError($"[ENTITY] Trying to destroy Entity {entityId} but it's NULL");
                    return;
                }

                m_RecycledEntityIds.Add(entityId, entityId);
                Destroy(entity.gameObject);
                if (enableDebug) {
                    Debug.Log($"[ENTITY] Entity being Destroyed : {entityId} ({entity.EntityAddress})");
                }
            }
        }

        public void DestroyEntity(GameEntity entity) {
            DestroyEntity(entity.EntityId);
        }

        public void UnregisterEntity(uint entityId) {
            lock (m_LockRoot) {
                if (!m_Entities.ContainsKey(entityId)) return;
                var entity = m_Entities[entityId];
                if (entity == null) {
                    Debug.LogError($"[ENTITY] Entity {entityId} can not be unregistered: NULL.");
                    m_Entities.Remove(entityId);
                    return;
                }

                entity.OnDestroyCallbackInternal();
                if (enableDebug) {
                    Debug.Log($"[ENTITY] Entity Unregistered : {entityId} ({entity.EntityAddress})");
                }

                var args = new OnEntityDestroyedEventArgs(entityId, entity.GetType(), entity.EntityAddress, entity.gameObject.name);
                Game.Event.Invoke(OnEntityDestroyedEvtName, entityId, args);
                m_Entities.Remove(entityId);
                m_RecycledEntityIds.Remove(entityId); // release recycled id if exist
            }
        }

        public void ChangeEntityOwner(uint entityId, uint ownerId, bool setParent = true, Transform root = null) {
            if (!m_Entities.ContainsKey(entityId)) {
                return;
            }

            if (!m_Entities.ContainsKey(ownerId)) {
                return;
            }

            m_Entities[entityId].OwnerId = ownerId;
            if (setParent) {
                if (root) {
                    m_Entities[entityId].transform.SetParent(root);
                }
                else {
                    m_Entities[entityId].transform.SetParent(m_Entities[ownerId].transform);
                }
            }

            Game.Event.Invoke(OnEntityOwnerChangedEvtName, entityId, ownerId);
        }

        public T GetEntity<T>(uint entityId) where T : GameEntity {
            if (!m_Entities.ContainsKey(entityId)) {
                Debug.LogWarning($"[ENTITY] Entity : {entityId} is not existed or is not Type of {typeof(T)}");
                return null;
            }

            var result = m_Entities[entityId];
            return result as T;
        }

        public GameEntity GetEntity(uint entityId) {
            return GetEntity<GameEntity>(entityId);
        }

        public bool IsEntityAvailable(uint entityId) {
            lock (m_LockRoot) {
                return m_Entities.ContainsKey(entityId) && !m_RecycledEntityIds.ContainsKey(entityId);
            }
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
                entity.OnDestroyCallbackInternal();
                Destroy(entity.gameObject);
            }

            m_Entities.Clear();
        }

        private uint CalculateFinalId(uint presetId) {
            uint finalId = 1;
            if (presetId != 0) {
                finalId = presetId;
            }
            else {
                while (m_Entities.ContainsKey(finalId) || m_RecycledEntityIds.ContainsKey(finalId)) {
                    finalId++;
                }
            }

            return finalId;
        }

        private void SetUpGameEntity<T>(T entity, string address, uint presetId, uint ownerEntityId, bool setParent, Action<T> onInstantiated = null) where T : GameEntity {
            if (presetId != 0) DestroyEntity(presetId);
            var finalId = CalculateFinalId(presetId);

            entity.EntityId = finalId;
            entity.EntityAddress = address;
            entity.gameObject.name = entity.EntityAddress;
            if (ownerEntityId != 0) {
                if (IsEntityAvailable(ownerEntityId)) {
                    entity.OwnerId = ownerEntityId;
                    if (setParent) {
                        entity.transform.SetParent(GetEntity<GameEntity>(ownerEntityId).transform, false);
                    }
                }
                else {
                    entity.OwnerId = 0;
                    Debug.LogWarning("[ENTITY] Entity OwnerId : " + ownerEntityId + $" is not existed. Entity {entity.name} ownerId set to 0(System Owner)");
                }
            }

            m_Entities.Add(finalId, entity);

            onInstantiated?.Invoke(entity);

            var args = new OnEntityInstantiatedEventArgs(finalId, entity.EntityAddress, entity.gameObject.name);

            entity.OnInitCallbackInternal();
            Game.Event.InvokeNow(OnEntityInstantiatedEvtName, finalId, args);
        }

        protected override void Awake() {
            base.Awake();
            Game.Entity = this;
        }
    }
}