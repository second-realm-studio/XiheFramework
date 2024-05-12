using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using XiheFramework.Core.Base;

namespace XiheFramework.Core.Config {
    public class ConfigModule : GameModule {
        public TextAsset configJsonAsset;
        private Dictionary<string, ConfigEntry> m_ConfigEntries = new Dictionary<string, ConfigEntry>();

        public void AddConfig(ConfigEntry configEntry) {
            if (!m_ConfigEntries.ContainsKey(configEntry.path)) {
                m_ConfigEntries.Add(configEntry.path, configEntry);
                Debug.Log($"Add Config: {configEntry.path} to {configEntry.value}");
            }
            else {
                m_ConfigEntries[configEntry.path] = configEntry;
                Debug.Log($"Update Config: {configEntry.path} to {configEntry.value}");
            }
        }

        public void RemoveConfig(string configPath) {
            if (m_ConfigEntries.ContainsKey(configPath)) {
                m_ConfigEntries.Remove(configPath);
                Debug.Log($"Remove Config: {configPath}");
            }
        }

        public T FetchConfig<T>(string configPath) {
            if (m_ConfigEntries.TryGetValue(configPath, out var entry)) {
                return entry.value is T value ? value : default;
            }

            return default;
        }

        public override void Setup() {
            var data = JsonUtility.FromJson<ConfigData>(configJsonAsset.text);
            foreach (var setting in data.entries) {
                AddConfig(setting);
            }
        }
    }
}