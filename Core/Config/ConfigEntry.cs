using System;

namespace XiheFramework.Core.Config {
    [Serializable]
    public class ConfigEntry {
        public Type configType;
        public string configPath;
        public object configValue;

        public ConfigEntry() { }

        public ConfigEntry(Type configType, string configPath, object configValue) {
            this.configType = configType;
            this.configPath = configPath;
            this.configValue = configValue;
        }
    }
}