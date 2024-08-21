using System;
using UnityEngine;
using XiheFramework.Combat.Damage.Interfaces;
using XiheFramework.Core.Entity;
using XiheFramework.Core.LogicTime;
using XiheFramework.Core.Utility.Extension;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Damage.HitBox {
    public abstract class HitBoxBase : MonoBehaviour {
        public Action<IDamageData> OnDamageDealt { get; set; }
        public Action<Collider> OnHit { get; set; }
        public uint OwnerId { get; private set; }

        public Collider hitBoxCollider;
        public LayerMask damageLayerMask;
        public LayerMask hitLayerMask;

        protected abstract void OnContactCallback(Collider other);
        protected abstract void OnDamageDealtCallback(IDamageData damageData);
        protected abstract IDamageData GetDamageData(uint senderId, uint receiverId);

        public virtual void EnableHitBox(uint owner) {
            OwnerId = owner;
            hitBoxCollider.enabled = true;
        }

        public virtual void DisableHitBox() {
            if (hitBoxCollider != null) {
                hitBoxCollider.enabled = false;
            }
        }

        public void ClearOnHit() {
            OnHit = null;
        }

        public void ClearOnDamageDealt() {
            OnDamageDealt = null;
        }

        private void Awake() {
            if (hitBoxCollider == null) {
                hitBoxCollider = GetComponent<Collider>();
            }

            hitBoxCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider other) {
            if (hitLayerMask.Includes(other.gameObject.layer)) {
                OnContactCallback(other);
                OnHit?.Invoke(other);
            }

            var hurtBox = other.GetComponentInParent<HurtBox>();

            if (hurtBox == null) {
                return;
            }

            if (damageLayerMask.Includes(other.gameObject.layer)) {
                var data = GetDamageData(OwnerId, hurtBox.owner.EntityId);
                OnDamageDealtCallback(data);
                OnDamageDealt?.Invoke(data);
                Game.Damage.RegisterDamage(data);
            }
        }

#if UNITY_EDITOR
        private void OnValidate() {
            if (hitBoxCollider == null) {
                hitBoxCollider = GetComponent<Collider>();
            }
        }
#endif
    }
}