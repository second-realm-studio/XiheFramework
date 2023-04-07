using UnityEditor;
using UnityEngine;

namespace XiheFramework.Utility.Playables.TextSwitcher.Editor {
    [CustomPropertyDrawer(typeof(TextSwitcherBehaviour))]
    public class TextSwitcherDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var fieldCount = 5;
            return fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var colorProp = property.FindPropertyRelative("color");
            var fontSizeProp = property.FindPropertyRelative("fontSize");
            var speedProp = property.FindPropertyRelative("speed");
            var textProp = property.FindPropertyRelative("text");
            var localizedProp = property.FindPropertyRelative("localized");

            var singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(singleFieldRect, colorProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, fontSizeProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, speedProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, textProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, localizedProp);
        }
    }
}