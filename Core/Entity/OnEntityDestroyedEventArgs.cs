using System;
using UnityEngine;
using XiheFramework.Core.Event;

namespace XiheFramework.Core.Entity {
    public struct OnEntityDestroyedEventArgs {
        public uint entityId;
        public Type entityType;
        public string entityAddress;
        public string gameObjectName;
        public Vector3 positionBeforeDestroy;
        public Quaternion rotationBeforeDestroy;

        public OnEntityDestroyedEventArgs(uint entityId, Type entityType, string entityAddress, string gameObjectName, Vector3 positionBeforeDestroy,
            Quaternion rotationBeforeDestroy) {
            this.entityId = entityId;
            this.entityType = entityType;
            this.entityAddress = entityAddress;
            this.gameObjectName = gameObjectName;
            this.positionBeforeDestroy = positionBeforeDestroy;
            this.rotationBeforeDestroy = rotationBeforeDestroy;
        }
    }
}