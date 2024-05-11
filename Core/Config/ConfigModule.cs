using System;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core.Base;

namespace XiheFramework.Core.Config {
    public class ConfigModule : GameModule {
        public LinkedList<ConfigSetting> configSettings = new LinkedList<ConfigSetting>();

        private Dictionary<string, ConfigEntry> m_ConfigEntries = new Dictionary<string, ConfigEntry>();

        public void AddConfig(ConfigEntry configEntry) {
            if (m_ConfigEntries.ContainsKey(configEntry.configPath)) {
                m_ConfigEntries[configEntry.configPath] = configEntry;
            }
            else {
                m_ConfigEntries.Add(configEntry.configPath, configEntry);
                Debug.Log($"Add Config: {configEntry.configPath}");
            }
        }

        public void RemoveConfig(string configPath) {
            if (m_ConfigEntries.ContainsKey(configPath)) {
                m_ConfigEntries.Remove(configPath);
                Debug.Log($"Remove Config: {configPath}");
            }
        }

        public T FetchConfig<T>(string configPath) where T : ConfigEntry {
            if (m_ConfigEntries.TryGetValue(configPath, out var entry)) {
                return entry as T;
            }

            return null;
        }

        public override void Setup() {
            foreach (var setting in configSettings) {
                var entry = new ConfigEntry(setting.configType, setting.configPath, setting.configValue);
                AddConfig(entry);
            }
        }

        [Serializable]
        public class ConfigSetting {
            public Type configType;
            public string configPath;
            public object configValue;
        }
    }
}