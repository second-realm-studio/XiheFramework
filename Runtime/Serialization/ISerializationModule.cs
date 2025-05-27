namespace XiheFramework.Runtime.Serialization {
    public interface ISerializationModule {
        public string OnSaveEventName { get; }
        public string OnLoadEventName { get; }
        public void SaveGame();
        public void LoadGame();
    }
}