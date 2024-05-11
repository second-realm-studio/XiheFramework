using System;
using UnityEngine;
using XiheFramework.Combat.Base;
using XiheFramework.Combat.Damage.DataTypes;
using XiheFramework.Core.Entity;
using XiheFramework.Core.LogicTime;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Damage.HitBox {
    public abstract class HitBoxEntityBase : TimeBasedGameEntity {
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

        public void SetHitCallback(Action<int> onHitCallback) {
            onHit = onHitCallback;
        }

        public void SetDamageCallback(Action<DamageEventArgs> onDamageCallback) {
            m_OnDamaged = onDamageCallback;
        }

        public virtual void ActivateHitBox() {
            hitBoxCollider.enabled = true;

            if (!String.IsNullOrEmpty(m_OnProcessedDamageEventHandlerId)) {
                Game.Event.Unsubscribe(Game.Damage.onProcessedDamageEventName, m_OnProcessedDamageEventHandlerId);
            }

            m_OnProcessedDamageEventHandlerId = Game.Event.Subscribe(Game.Damage.onProcessedDamageEventName, OnProcessedDamage);
        }

        public virtual void DeactivateHitBox() {
            if (!String.IsNullOrEmpty(m_OnProcessedDamageEventHandlerId)) {
                Game.Event.Unsubscribe(Game.Damage.onProcessedDamageEventName, m_OnProcessedDamageEventHandlerId);
            }

            if (hitBoxCollider != null) {
                hitBoxCollider.enabled = false;
            }
        }

        public void ClearOnDamagedCallback() {
            m_OnDamaged = null;
        }

        public override void OnInitCallback() {
            base.OnInitCallback();

            if (hitBoxCollider == null) {
                hitBoxCollider = GetComponent<Collider>();
            }

            hitBoxCollider.enabled = false;
        }

        public override void OnDestroyCallback() {
            base.OnDestroyCallback();

            if (!String.IsNullOrEmpty(m_OnProcessedDamageEventHandlerId)) {
                Game.Event.Unsubscribe(Game.Damage.onProcessedDamageEventName, m_OnProcessedDamageEventHandlerId);
            }
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
    }
}