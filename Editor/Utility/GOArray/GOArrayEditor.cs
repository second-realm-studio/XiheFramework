using UnityEditor;
using UnityEngine;

namespace XiheFramework.Editor.Utility.GoArray {
    [CustomEditor(typeof(GOArray))]
    public class GOArrayEditor : UnityEditor.Editor {
        private GOArray m_Target;

        private void OnEnable() {
            m_Target = (GOArray)target;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (GUILayout.Button("Update")) m_Target.UpdateModifier();

            if (GUILayout.Button("Clear")) m_Target.ClearModifier();
        }
    }
}