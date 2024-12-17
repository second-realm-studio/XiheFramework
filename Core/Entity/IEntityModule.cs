using System;
using UnityEngine;

namespace XiheFramework.Core.Entity {
    public interface IEntityModule {
        public string OnEntityInstantiatedEvtName { get; }
        public string OnEntityDestroyedEvtName { get; }
        public string OnEntityOwnerChangedEvtName { get; }


        public T InstantiateEntity<T>(string entityAddress, Vector3 localPosition, Quaternion localRotation, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0,
            Action<T> onInstantiatedCallback = null) where T : GameEntity;

        public T InstantiateEntity<T>(string entityAddress, Vector3 localPosition, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0,
            Action<T> onInstantiatedCallback = null) where T : GameEntity;

        public T InstantiateEntity<T>(string entityAddress, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0, Action<T> onInstantiatedCallback = null)
            where T : GameEntity;

        public GameEntity InstantiateEntity(string entityAddress, Vector3 localPosition, Quaternion localRotation, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0,
            Action<GameEntity> onInstantiatedCallback = null);

        public GameEntity InstantiateEntity(string entityAddress, Vector3 localPosition, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0,
            Action<GameEntity> onInstantiatedCallback = null);

        public GameEntity InstantiateEntity(string entityAddress, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0,
            Action<GameEntity> onInstantiatedCallback = null);

        public void InstantiateEntityAsync<T>(string entityAddress, Vector3 localPosition, Quaternion localRotation, uint ownerEntityId = 0, bool setParent = true,
            uint presetId = 0, Action<T> onInstantiatedCallback = null) where T : GameEntity;

        public void InstantiateEntityAsync<T>(string entityAddress, Vector3 localPosition, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0,
            Action<T> onInstantiatedCallback = null) where T : GameEntity;

        public void InstantiateEntityAsync<T>(string entityAddress, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0, Action<T> onInstantiatedCallback = null)
            where T : GameEntity;

        public void DestroyEntity(uint entityId);
        
        public void DestroyEntity(GameEntity entity);
        
        public void RegisterInstantiatedEntity<T>(T entity, uint ownerEntityId = 0, bool setParent = true, uint presetId = 0) where T : GameEntity;
        
        public void UnregisterEntity(uint entityId);
        
        public void ChangeEntityOwner(uint entityId, uint ownerId, bool setParent = true, Transform root = null);

        public T GetEntity<T>(uint entityId) where T : GameEntity;

        public GameEntity GetEntity(uint entityId);

        public bool IsEntityAvailable(uint entityId);
    }
}