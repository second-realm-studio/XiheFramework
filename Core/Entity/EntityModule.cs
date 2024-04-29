using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core.Base;

namespace XiheFramework.Core.Entity {
    //TODO: change to hash based id
    public class EntityModule : GameModule {
        public readonly string OnEntityRegisteredEventName = "Event.OnEntityRegistered";
        private uint m_NextId = 1001;
        private readonly Dictionary<uint, GameEntity> m_Entities = new();

        private readonly object m_LockRoot = new();

        public void RegisterEntity(GameEntity entity, out uint distributedId, uint presetId = 0) {
            lock (m_LockRoot) {
                if (presetId != 0) {
                    if (m_Entities.ContainsKey(presetId)) {
                        Debug.LogWarning($"[ENTITY] Preset Id {presetId} is already existed, replacing it, make sure the last instance with this id is destroyed");
                        m_Entities[presetId] = entity;
                    }
                    else {
                        m_Entities.Add(presetId, entity);
                    }

                    distributedId = presetId;
                    GameCore.Event.InvokeNow(OnEntityRegisteredEventName, entity, presetId);
                    return;
                }

                while (m_Entities.ContainsKey(m_NextId)) {
                    m_NextId++;
                }

                distributedId = m_NextId;
                m_Entities.Add(m_NextId, entity);
                GameCore.Event.InvokeNow(OnEntityRegisteredEventName, entity, m_NextId);
            }
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

        public override void OnReset() {
            m_Entities.Clear();
            m_NextId = 1001;
        }
    }
}