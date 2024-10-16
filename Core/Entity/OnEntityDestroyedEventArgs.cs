using System;
using XiheFramework.Core.Event;

namespace XiheFramework.Core.Entity {
    public struct OnEntityDestroyedEventArgs{
        public uint entityId;
        public Type entityType;
        public string entityAddress;
        public string gameObjectName;

        public OnEntityDestroyedEventArgs(uint entityId, Type entityType, string entityAddress, string gameObjectName) {
            this.entityId = entityId;
            this.entityType = entityType;
            this.entityAddress = entityAddress;
            this.gameObjectName = gameObjectName;
        }
    }
}