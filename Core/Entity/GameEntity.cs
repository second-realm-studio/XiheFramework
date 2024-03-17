using UnityEngine;
using XiheFramework.Entry;

namespace XiheFramework.Core.Entity {
    public abstract class GameEntity : MonoBehaviour {
        public uint presetId = 0; //0 means no preset
        protected uint entityId;

        public uint EntityId => entityId;

        public uint GetEntityId() {
            return entityId;
        }

        internal void SetEntityId(uint id) {
            entityId = id;
        }

        protected virtual void Start() {
            Game.Entity.RegisterEntity(this, out entityId, presetId);
        }
    }
}