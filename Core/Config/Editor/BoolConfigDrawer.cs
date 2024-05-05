using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using XiheFramework.Core.Config.Entry;

namespace XiheFramework.Core.Config.Editor {
    [CustomPropertyDrawer(typeof(BoolConfigEntry))]
    public class BoolConfigDrawer : PropertyDrawer {
        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            // Create property container element.
            var container = new VisualElement();

            // Create property fields.
            var amountField = new PropertyField(property.FindPropertyRelative("value"));

            container.Add(amountField);

            return container;
        }
    }
}