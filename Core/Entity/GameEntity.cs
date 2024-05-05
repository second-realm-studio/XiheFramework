using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace XiheFramework.Core.Entity {
    public abstract class GameEntity : MonoBehaviour {
        public int executionOrder;

        public uint EntityId { get; internal set; }
        public uint OwnerId { get; internal set; }

        public virtual void OnInitCallback() { }
        public virtual void OnUpdateCallback() { }
        public virtual void OnFixedUpdateCallback() { }
        public virtual void OnLateUpdateCallback() { }
        public virtual void OnDestroyCallback() { }

        // private void Start() { }
        // private void Update() { }
        // private void FixedUpdate() { }
        // private void LateUpdate() { }
        // private void OnDestroy() { }
    }
}