using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using XiheFramework.Core.Config.Entry;

namespace XiheFramework.Core.Config.Editor {
    [CustomEditor(typeof(ConfigModule))]
    public class ConfigModuleEditor : UnityEditor.Editor {
        private ConfigModule m_Target;

        private string[] m_ConfigEntryTypes;

        private void OnEnable() {
            m_Target = (ConfigModule)target;
            m_ConfigEntryTypes = GetAllConfigEntryTypes();
        }

        public override void OnInspectorGUI() {
            LinkedList<ConfigModule.ConfigSetting> list = m_Target.configSettings;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.MinWidth(75));
            EditorGUILayout.LabelField("Type", EditorStyles.boldLabel, GUILayout.Width(75));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.MinWidth(100));
            EditorGUILayout.LabelField("Path", EditorStyles.boldLabel, GUILayout.Width(100));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.MinWidth(100));
            EditorGUILayout.LabelField("Value", EditorStyles.boldLabel, GUILayout.Width(100));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.MinWidth(30));
            EditorGUILayout.EndVertical();


            EditorGUILayout.EndHorizontal();

            for (LinkedListNode<ConfigModule.ConfigSetting> node = m_Target.configSettings.First; node != null; node = node.Next) {
                EditorGUILayout.BeginHorizontal();

                //type
                var selectedId = 0;
                if (!string.IsNullOrEmpty(node.Value.configType)) {
                    if (m_ConfigEntryTypes.Contains(node.Value.configType)) {
                        selectedId = Array.IndexOf(m_ConfigEntryTypes, node.Value.configType);
                    }
                }

                EditorGUILayout.BeginVertical(GUILayout.MinWidth(75));
                selectedId = EditorGUILayout.Popup(selectedId, m_ConfigEntryTypes);
                EditorGUILayout.EndVertical();

                node.Value.configType = m_ConfigEntryTypes[selectedId];

                //path
                EditorGUILayout.BeginVertical(GUILayout.MinWidth(100));
                node.Value.configEntry.configPath = EditorGUILayout.TextField(node.Value.configEntry.configPath);
                EditorGUILayout.EndVertical();

                //value
                EditorGUILayout.BeginVertical(GUILayout.MinWidth(100));
                Debug.Log(node.Value.configType);

                switch (node.Value.configType) {
                    case "Bool":
                        node.Value.configEntry = new BoolConfigEntry();
                        var boolEntry = (BoolConfigEntry)(node.Value.configEntry);
                        boolEntry.value = EditorGUILayout.Toggle(boolEntry.value);
                        break;
                    case "Float":
                        node.Value.configEntry = new FloatConfigEntry();
                        var floatEntry = (FloatConfigEntry)(node.Value.configEntry);
                        floatEntry.value = EditorGUILayout.FloatField(floatEntry.value);
                        break;
                    case "Int":
                        node.Value.configEntry = new IntConfigEntry();
                        var intEntry = (IntConfigEntry)(node.Value.configEntry);
                        intEntry.value = EditorGUILayout.IntField(intEntry.value);
                        break;
                    case "Object":
                        node.Value.configEntry = new ObjectConfigEntry();
                        var objectEntry = (ObjectConfigEntry)(node.Value.configEntry);
                        objectEntry.value = EditorGUILayout.ObjectField((UnityEngine.Object)objectEntry.value, typeof(UnityEngine.Object), true);
                        break;
                    case "String":
                        node.Value.configEntry = new StringConfigEntry();
                        var stringEntry = (StringConfigEntry)(node.Value.configEntry);
                        stringEntry.value = EditorGUILayout.TextField(stringEntry.value);
                        break;
                    case "Vector2":
                        node.Value.configEntry = new Vector2ConfigEntry();
                        var vector2Entry = (Vector2ConfigEntry)(node.Value.configEntry);
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(10));
                        EditorGUILayout.LabelField("X", GUILayout.Width(10));
                        vector2Entry.x = EditorGUILayout.FloatField(vector2Entry.x);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(10));
                        EditorGUILayout.LabelField("Y", GUILayout.Width(10));
                        vector2Entry.y = EditorGUILayout.FloatField(vector2Entry.y);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                        break;
                    default:
                        break;
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUILayout.MinWidth(30));
                if (GUILayout.Button("\u00d7")) {
                    list.Remove(node);
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }

            m_Target.configSettings = list;

            if (GUILayout.Button("Add Config")) {
                m_Target.configSettings.AddLast(new ConfigModule.ConfigSetting());
            }

            if (GUILayout.Button("Refresh Types")) {
                m_ConfigEntryTypes = GetAllConfigEntryTypes();
            }
        }

        private string[] GetAllConfigEntryTypes() {
            var types = new List<string>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
                var assemblyTypes = assembly.GetTypes();
                foreach (var type in assemblyTypes) {
                    if (type.IsSubclassOf(typeof(ConfigEntryBase))) {
                        var fullName = type.Name;
                        var displayName = fullName.Replace("ConfigEntry", "");
                        types.Add(displayName);
                    }
                }
            }

            return types.ToArray();
        }

        private string TrimConfigTypeName(string typeName) {
            return typeName.Replace("ConfigEntry", "");
        }
    }
}