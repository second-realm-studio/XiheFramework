using System.Collections.Generic;
using XiheFramework.Core.Base;
using XiheFramework.Core.Config.Entry;

namespace XiheFramework.Core.Config {
    public class ConfigModule : GameModule {
        public LinkedList<ConfigSetting> configSettings = new LinkedList<ConfigSetting>();

        private Dictionary<string, ConfigEntryBase> m_ConfigEntries = new Dictionary<string, ConfigEntryBase>();

        public void AddConfig(ConfigEntryBase configEntry) {
            if (m_ConfigEntries.ContainsKey(configEntry.configPath)) {
                m_ConfigEntries[configEntry.configPath] = configEntry;
            }
            else {
                m_ConfigEntries.Add(configEntry.configPath, configEntry);
            }
        }

        public void RemoveConfig(string configPath) {
            if (m_ConfigEntries.ContainsKey(configPath)) {
                m_ConfigEntries.Remove(configPath);
            }
        }

        public T FetchConfig<T>(string configPath) where T : ConfigEntryBase {
            if (m_ConfigEntries.TryGetValue(configPath, out var entry)) {
                return entry as T;
            }

            return null;
        }

        public class ConfigSetting {
            public string configType = nameof(StringConfigEntry);
            public ConfigEntryBase configEntry = new StringConfigEntry();
        }
    }
}