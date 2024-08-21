using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Config {
    public class ConfigModule : GameModule {
        public List<PresetConfigEntry> configSettings = new List<PresetConfigEntry>();

        private Dictionary<string, ConfigEntry> m_ConfigEntries = new Dictionary<string, ConfigEntry>();

        public void AddConfig(ConfigEntry configEntry) {
            if (!m_ConfigEntries.ContainsKey(configEntry.path)) {
                m_ConfigEntries.Add(configEntry.path, configEntry);
                if (enableDebug) {
                    Debug.Log($"[CFG]Add Config: {configEntry.path} to {configEntry.value}");
                }
            }
            else {
                var oldType = m_ConfigEntries[configEntry.path].type;
                var oldValue = m_ConfigEntries[configEntry.path].value;
                m_ConfigEntries[configEntry.path] = configEntry;
                Debug.LogWarning(
                    $"[CFG]Duplicated Config. Replacing Config:\"{configEntry.path}\" from [{oldType}] {oldValue} to [{configEntry.type}] {configEntry.value}");
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
            foreach (var setting in configSettings) {
                var newEntry = new ConfigEntry {
                    path = setting.path,
                    type = setting.type,
                    value = setting.GetValue()
                };
                AddConfig(newEntry);
            }

            if (enableDebug) {
                Debug.Log($"[CONFIG]Loaded {m_ConfigEntries.Count} config entries");
            }
        }

        protected override void Awake() {
            base.Awake();

            Game.Config = this;
        }
    }
}