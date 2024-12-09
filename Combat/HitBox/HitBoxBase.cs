using UnityEngine;

namespace XiheFramework.Combat.HitBox {
    public abstract class HitBoxBase : MonoBehaviour {
        public uint OwnerId { get; private set; }

        public LayerMask hitLayerMask;

        public void EnableHitBox(uint owner) {
            OwnerId = owner;
            OnEnableHitBox();
        }

        public void DisableHitBox() => OnDisableHitBox();

        protected abstract void OnEnableHitBox();

        protected abstract void OnDisableHitBox();
    }
}