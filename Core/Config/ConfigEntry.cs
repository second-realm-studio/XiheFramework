using System;

namespace XiheFramework.Core.Config {
    [Serializable]
    public class ConfigEntry {
        public string path;
        public ConfigType type;
        public string value;
    }
}