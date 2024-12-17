using System;
using UnityEngine;

namespace XiheFramework.Core.Serialization.Serializable {
    [Serializable]
    public class SerializableEntityData {
        public string entityAddress;
        public Vector3 position;
        public Vector3 rotation;
        public string customData;
    }
}