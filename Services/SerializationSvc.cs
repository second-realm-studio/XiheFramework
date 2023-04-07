using XiheFramework.Modules.Base;

namespace XiheFramework.Services {
    public static class SerializationSvc {
        public static void WriteSaveData(string fileName) {
            Game.Serialization.WriteSaveData(fileName);
        }

        public static void ReadSaveData(string fileName) {
            Game.Serialization.ReadSaveData(fileName);
        }
    }
}