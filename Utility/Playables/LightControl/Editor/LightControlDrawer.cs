using UnityEditor;
using UnityEngine;

namespace XiheFramework.Utility.Playables.LightControl.Editor {
    [CustomPropertyDrawer(typeof(LightControlBehaviour))]
    public class LightControlDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var fieldCount = 4;
            return fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var colorProp = property.FindPropertyRelative("color");
            var intensityProp = property.FindPropertyRelative("intensity");
            var bounceIntensityProp = property.FindPropertyRelative("bounceIntensity");
            var rangeProp = property.FindPropertyRelative("range");

            var singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(singleFieldRect, colorProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, intensityProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, bounceIntensityProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, rangeProp);
        }
    }
}