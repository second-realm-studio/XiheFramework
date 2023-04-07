using UnityEditor;
using UnityEngine;

namespace XiheFramework.Utility.Playables.ScreenFader.Editor {
    [CustomPropertyDrawer(typeof(ScreenFaderBehaviour))]
    public class ScreenFaderDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var fieldCount = 1;
            return fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var colorProp = property.FindPropertyRelative("color");

            var singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(singleFieldRect, colorProp);
        }
    }
}