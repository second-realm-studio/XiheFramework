namespace XiheFramework.Core.Entity {
    public struct OnEntityInstantiatedEventArgs {
        public uint entityId;
        public string entityName;
        public string gameObjectName;
        public OnEntityInstantiatedEventArgs(uint entityId, string entityName, string gameObjectName) {
            this.entityId = entityId;
            this.entityName = entityName;
            this.gameObjectName = gameObjectName;
        }
    }
}