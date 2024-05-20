using System;
using UnityEditor;
using UnityEngine;
using XiheFramework.Core.Config;

namespace XiheFramework.Editor.Core.Config {
    [CustomEditor(typeof(ConfigModule))]
    public class ConfigModuleEditor : UnityEditor.Editor {
        private SerializedObject m_Target;

        private void OnEnable() {
            var p = (ConfigModule)target;
            m_Target = new SerializedObject(p);
        }

        public override void OnInspectorGUI() {
            m_Target.Update();
            EditorGUILayout.PropertyField(m_Target.FindProperty("updateInterval"));
            EditorGUILayout.PropertyField(m_Target.FindProperty("fixedUpdateInterval"));
            EditorGUILayout.PropertyField(m_Target.FindProperty("lateUpdateInterval"));
            EditorGUILayout.PropertyField(m_Target.FindProperty("enableDebug"));
            m_Target.ApplyModifiedProperties();

            if (GUILayout.Button("Open Config Editor", GUILayout.Height(50))) {
                ConfigEditorWindow.ShowWindow(m_Target);
            }
        }
    }
}