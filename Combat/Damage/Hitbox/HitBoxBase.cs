using System;
using UnityEngine;
using XiheFramework.Combat.Base;
using XiheFramework.Combat.Damage.DataTypes;
using XiheFramework.Entry;

namespace XiheFramework.Combat.Damage.Hitbox {
    public abstract class HitBoxBase : MonoBehaviour {
        public float healthDamage;
        public float staminaDamage;
        public Vector3 forceDir;
        public float forceMagnitude;
        public RawDamageType rawDamageType;
        public float stunDuration;
        public Collider hitBoxCollider;
        public string[] damageTags;

        public LayerMask damageLayerMask;
        public LayerMask hitLayerMask;

        private Action<DamageEventArgs> m_OnDamaged;
        protected Action<int> onHit;

        private string m_OnProcessedDamageEventHandlerId;

        public uint OwnerId { get; private set; }

        private void Awake() {
            if (hitBoxCollider == null) {
                hitBoxCollider = GetComponent<Collider>();
            }

            hitBoxCollider.enabled = false;
        }

        public void SetOwnerId(CombatEntity owner) {
            OwnerId = owner.GetEntityId();
        }

        public void SetOwnerId(uint ownerId) {
            OwnerId = ownerId;
        }

        public void SetHitCallback(Action<int> onHitCallback) {
            onHit = onHitCallback;
        }

        public void SetDamageCallback(Action<DamageEventArgs> onDamageCallback) {
            m_OnDamaged = onDamageCallback;
        }

        public virtual void ActivateDamageSender() {
            hitBoxCollider.enabled = true;

            if (!String.IsNullOrEmpty(m_OnProcessedDamageEventHandlerId)) {
                XiheFramework.Entry.Game.Event.Unsubscribe(Game.Damage.OnProcessedDamageEventName, m_OnProcessedDamageEventHandlerId);
            }

            m_OnProcessedDamageEventHandlerId = XiheFramework.Entry.Game.Event.Subscribe(Game.Damage.OnProcessedDamageEventName, OnProcessedDamage);
        }

        public virtual void DeactivateDamageSender() {
            if (!String.IsNullOrEmpty(m_OnProcessedDamageEventHandlerId)) {
                XiheFramework.Entry.Game.Event.Unsubscribe(Game.Damage.OnProcessedDamageEventName, m_OnProcessedDamageEventHandlerId);
            }

            if (hitBoxCollider != null) {
                hitBoxCollider.enabled = false;
            }
        }

        public void ClearOnDamagedCallback() {
            m_OnDamaged = null;
        }

        private void OnProcessedDamage(object sender, object e) {
            if (sender is not uint) {
                return;
            }

            var args = (DamageEventArgs)e;
            if (args.senderId == OwnerId) {
                m_OnDamaged?.Invoke(args);
            }
        }

        private void OnDestroy() {
            if (XiheFramework.Entry.Game.Event != null) {
                if (!String.IsNullOrEmpty(m_OnProcessedDamageEventHandlerId)) {
                    XiheFramework.Entry.Game.Event.Unsubscribe(Game.Damage.OnProcessedDamageEventName, m_OnProcessedDamageEventHandlerId);
                }
            }
        }
    }
}