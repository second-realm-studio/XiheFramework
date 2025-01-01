using System;
using UnityEngine;

namespace XiheFramework.Core.Config {
    [Serializable]
    public class PresetConfigEntry {
        public string path;
        public ConfigType type;

        public int intValue;
        public bool boolValue;
        public float floatValue;
        public string stringValue;
        public Color colorValue;
        public LayerMask layerMaskValue;
        public Vector2 vector2Value;
        public Vector3 vector3Value;
        public Vector4 vector4Value;
        public AnimationCurve animationCurveValue;

        public object GetValue() {
            switch (type) {
                case ConfigType.Integer:
                    return intValue;
                case ConfigType.Boolean:
                    return boolValue;
                case ConfigType.Float:
                    return floatValue;
                case ConfigType.String:
                    return stringValue;
                case ConfigType.Color:
                    return colorValue;
                case ConfigType.LayerMask:
                    return layerMaskValue;
                case ConfigType.Vector2:
                    return vector2Value;
                case ConfigType.Vector3:
                    return vector3Value;
                case ConfigType.Vector4:
                    return vector4Value;
                case ConfigType.AnimationCurve:
                    return animationCurveValue;
                default:
                    return null;
            }
        }

        public void SetValue(object value) {
            switch (type) {
                case ConfigType.Integer:
                    intValue = (int)value;
                    break;
                case ConfigType.Boolean:
                    boolValue = (bool)value;
                    break;
                case ConfigType.Float:
                    floatValue = (float)value;
                    break;
                case ConfigType.String:
                    stringValue = (string)value;
                    break;
                case ConfigType.Color:
                    colorValue = (Color)value;
                    break;
                case ConfigType.LayerMask:
                    layerMaskValue = (LayerMask)value;
                    break;
                case ConfigType.Vector2:
                    vector2Value = (Vector2)value;
                    break;
                case ConfigType.Vector3:
                    vector3Value = (Vector3)value;
                    break;
                case ConfigType.Vector4:
                    vector4Value = (Vector4)value;
                    break;
                case ConfigType.AnimationCurve:
                    animationCurveValue = (AnimationCurve)value;
                    break;
                default:
                    Debug.LogError($"[CONFIG] Value: {value} Type: {value.GetType().FullName} cannot be set to Config Type: {type}");
                    break;
            }
        }
    }
}