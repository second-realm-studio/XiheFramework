using UnityEditor;
using UnityEngine;

namespace XiheFramework.Combat.Animation2D.LightMaskBaker.Scripts.Editor {
    public static class GUIHelper {
        public static T TextureField<T>(string name, T texture, float width = 70f, float height = 70f) where T : Texture {
            GUILayout.BeginVertical();
            var style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.UpperCenter;
            style.fixedWidth = width;
            GUILayout.Label(name, style);
            var result = (T)EditorGUILayout.ObjectField(texture ? texture : null, typeof(T), false, GUILayout.Width(width), GUILayout.Height(height));
            GUILayout.EndVertical();
            return result;
        }

        public static void DisplayTextureBox(string label, Texture texture, float width = 70f, float height = 70f) {
            if (texture) {
                TextureField(label, texture, width, height);
            }
            else {
                TextureField<Texture>(label, null, width, height);
            }
        }
    }
}