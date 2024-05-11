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

        /// <summary>
        /// instantiate entity
        /// </summary>
        /// <param name="entityAddress"> addressable address </param>
        /// <param name="ownerEntityId"> owner Id, 0 if no owner</param>
        /// <param name="setParent"></param>
        /// <param name="presetId"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T InstantiateEntity<T>(string entityAddress, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0) where T : GameEntity {
            lock (m_LockRoot) {
                var entity = Game.Resource.InstantiateAsset<GameObject>(entityAddress).GetComponent<T>();
                SetUpGameEntity(entity, presetId, ownerEntityId, setParent);
                return entity;
            }
        }

        public void InstantiateEntityAsync<T>(string entityAddress, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0, Action<T> onLoadCallback = null)
            where T : GameEntity {
            Game.Resource.InstantiateAssetAsync<GameObject>(entityAddress, go => {
                var entity = go.GetComponent<T>();
                SetUpGameEntity(entity, presetId, ownerEntityId, setParent);
                onLoadCallback?.Invoke(entity);
            });
        }

        public void DestroyEntity(uint entityId) {
            lock (m_LockRoot) {
                if (m_Entities.ContainsKey(entityId)) {
                    var entity = m_Entities[entityId];
                    entity.OnDestroyCallback();
                    Destroy(entity.gameObject);
                    m_Entities.Remove(entityId);
                    Game.Event.Invoke(onEntityDestroyedEvtName, entityId);
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
                return m_Entities[entityId] as T;
            }

            Debug.LogWarning($"[ENTITY] Entity : {entityId} is not Existed");
            return null;
        }

        public bool IsEntityExisted(uint entityId) {
            return m_Entities.ContainsKey(entityId);
        }

        public override void OnUpdate() {
            foreach (var entity in m_Entities) {
                entity.Value.OnUpdateCallback();
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

        void SetUpGameEntity(GameEntity entity, uint presetId, uint ownerEntityId, bool setParent) {
            uint finalId = 1;
            if (presetId != 0) {
                if (m_Entities.ContainsKey(presetId)) {
                    m_Entities[presetId].OnDestroyCallback();
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
            entity.EntityId = presetId;
            entity.OwnerId = ownerEntityId;
            if (setParent && ownerEntityId != 0) {
                entity.transform.SetParent(GetEntity<GameEntity>(ownerEntityId).transform, true);
            }

            entity.OnInitCallback();
            Game.Event.Invoke(onEntityInstantiatedEvtName, finalId);
        }
    }
}