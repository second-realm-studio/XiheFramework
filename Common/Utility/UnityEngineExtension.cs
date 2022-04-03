using System;
using System.Collections;
using System.Collections.Generic;
using FlowCanvas.Nodes;
using UnityEngine;

namespace XiheFramework {
    public static class UnityEngineExtension {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component {
            var component = gameObject.GetComponent<T>();
            if (component != null) {
                return component;
            }

            return gameObject.AddComponent<T>();
        }

        public static Vector2 ToVector2(this Vector3 vector3, V3ToV2Type convertType) {
            var result = convertType switch {
                V3ToV2Type.XY => new Vector2(vector3.x, vector3.y),
                V3ToV2Type.XZ => new Vector2(vector3.x, vector3.z),
                V3ToV2Type.YZ => new Vector2(vector3.y, vector3.z),
                _ => throw new ArgumentOutOfRangeException(nameof(convertType), convertType, null)
            };

            return result;
        }

        public static Vector3 ToVector3(this Vector2 vector2, V2ToV3Type convertType=V2ToV3Type.XZ) {
            var result = convertType switch {
                V2ToV3Type.XY => new Vector3(vector2.x, vector2.y, 0f),
                V2ToV3Type.XZ => new Vector3(vector2.x, 0f, vector2.y),
                V2ToV3Type.YZ => new Vector3(0f, vector2.x, vector2.y),
                _ => throw new ArgumentOutOfRangeException(nameof(convertType), convertType, null)
            };

            return result;
        }

        public static Vector2 Clamp(this Vector2 vector2, float min, float max) {
            float x = Mathf.Clamp(vector2.x, min, max);
            float y = Mathf.Clamp(vector2.y, min, max);

            return new Vector2(x, y);
        }

        public static Vector2 Clamp(this Vector2 vector2, Vector2 min, Vector2 max) {
            float x = Mathf.Clamp(vector2.x, min.x, max.x);
            float y = Mathf.Clamp(vector2.y, min.y, max.y);

            return new Vector2(x, y);
        }
    }

    public enum V3ToV2Type {
        XY,
        XZ,
        YZ
    }

    public enum V2ToV3Type {
        XY,
        XZ,
        YZ
    }
}