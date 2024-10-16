using System;
using UnityEditor;
using UnityEngine;
using XiheFramework.Core.Entity;

namespace XiheFramework.Editor.Core.Entity {
    [CustomEditor(typeof(GameEntity), true)]
    public class GameEntityEditor : UnityEditor.Editor {
        private GameEntity m_Target;
        private SerializedObject m_SerializedObject;

        private SerializedProperty m_PresetEntityId;
        private SerializedProperty m_PresetOwnerId;
        private SerializedProperty m_UseLogicTime;

        private void OnEnable() {
            m_Target = (GameEntity)target;
            m_SerializedObject = new SerializedObject(m_Target);

            m_PresetEntityId = m_SerializedObject.FindProperty("presetEntityId");
            m_PresetOwnerId = m_SerializedObject.FindProperty("presetOwnerId");
            m_UseLogicTime = m_SerializedObject.FindProperty("useLogicTime");
        }

        public override void OnInspectorGUI() {
            GUILayout.Label("Entity Status", EditorStyles.boldLabel);

            if (!Application.isPlaying) {
                GUI.enabled = false;
            }

            EditorGUILayout.LabelField("Entity ID", Application.isPlaying ? m_Target.EntityId.ToString() : "Not Playing");
            EditorGUILayout.LabelField("Owner ID", Application.isPlaying ? m_Target.OwnerId.ToString() : "Not Playing");
            EditorGUILayout.LabelField("Group Name", m_Target.GroupName);
            EditorGUILayout.LabelField("Full Name", Application.isPlaying ? m_Target.EntityFullName : "Not Playing");
            EditorGUILayout.LabelField("Time Scale", Application.isPlaying ? m_Target.TimeScale.ToString("0.000") : "Not Playing");

            GUI.enabled = true;
            GUILayout.Space(5f);

            GUILayout.Label("Entity Settings", EditorStyles.boldLabel);

            m_SerializedObject.Update();
            //isExpanded = static 
            GUILayout.BeginHorizontal();
            GUI.enabled = m_PresetEntityId.isExpanded;
            EditorGUILayout.PropertyField(m_PresetEntityId);
            GUI.enabled = true;
            if (Application.isPlaying) {
                GUI.enabled = false;
            }

            GUILayout.Label(new GUIContent("Dynamic", "Dynamic mode will let XiheFramework generate a dynamic Entity Id for this entity at runtime"), GUILayout.MaxWidth(55));
            m_PresetEntityId.isExpanded = !EditorGUILayout.Toggle(!m_PresetEntityId.isExpanded, GUILayout.MaxWidth(EditorGUIUtility.singleLineHeight));
            GUI.enabled = true;

            if (!m_PresetEntityId.isExpanded) {
                m_Target.presetEntityId = 0;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = m_PresetOwnerId.isExpanded;
            EditorGUILayout.PropertyField(m_PresetOwnerId);
            GUI.enabled = true;
            if (Application.isPlaying) {
                GUI.enabled = false;
            }

            GUILayout.Label(new GUIContent("Dynamic", "Dynamic mode will let XiheFramework assign a dynamic owner Id if this entity is instantiated with a owner ID at runtime"),
                GUILayout.MaxWidth(55));
            m_PresetOwnerId.isExpanded = !EditorGUILayout.Toggle(!m_PresetOwnerId.isExpanded, GUILayout.MaxWidth(EditorGUIUtility.singleLineHeight));
            GUI.enabled = true;

            if (!m_PresetOwnerId.isExpanded) {
                m_Target.presetOwnerId = 0;
            }

            GUILayout.EndHorizontal();


            EditorGUILayout.PropertyField(m_UseLogicTime);

            m_SerializedObject.ApplyModifiedProperties();

            GUILayout.Space(5f);
            DrawDefaultInspector();
        }
    }
}