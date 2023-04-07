using UnityEditor;
using UnityEngine;

namespace XiheFramework.Utility.Playables.TimeDilation.Editor {
    [CustomPropertyDrawer(typeof(TimeDilationBehaviour))]
    public class TimeDilationDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var fieldCount = 1;
            return fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var timeScaleProp = property.FindPropertyRelative("timeScale");

            var singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(singleFieldRect, timeScaleProp);
        }
    }
}