using UnityEngine;
using UnityEngine.Serialization;
using XiheFramework.Combat.Base;
using XiheFramework.Combat.Damage.DataTypes;
using XiheFramework.Combat.Damage.HitBox;
using XiheFramework.Core.LogicTime;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Projectile {
    public abstract class ProjectileEntity : TimeBasedGameEntity {
        public HitBoxEntityBase hitBox;
        public float lifeTime = 5f;

        public Vector3 StartPoint { get; private set; }
        public Vector3 EndPoint { get; private set; }

        private bool m_IsAirborne;
        private float m_LifeTimer;

        public void Launch(Vector3 startPoint, Vector3 endPoint, System.Action<DamageEventArgs> onDamageCallback) {
            System.Action<int> action = _ => { m_IsAirborne = false; };
            action += OnContact;
            hitBox.SetHitCallback(action);

            StartPoint = startPoint;
            EndPoint = endPoint;

            hitBox.DeactivateHitBox();
            onDamageCallback += OnDamage;
            hitBox.SetDamageCallback(onDamageCallback);
            hitBox.ActivateHitBox();

            OnLaunch();

            m_IsAirborne = true;
            m_LifeTimer = 0f;
        }

        public override void OnUpdateCallback() {
            if (!m_IsAirborne) {
                return;
            }

            m_LifeTimer += ScaledDeltaTime;
            OnAirborne(m_LifeTimer);

            if (m_LifeTimer >= lifeTime) {
                Game.Projectile.DestroyProjectile(EntityId);
            }
        }
        
        public override void OnDestroyCallback() {
            base.OnDestroyCallback();
            
            StopAllCoroutines();
        }

        /// <summary>
        /// Calls on the first frame when the projectile is launched
        /// </summary>
        protected abstract void OnLaunch();

        /// <summary>
        /// Calls every frame when the projectile is in the air
        /// </summary>
        protected abstract void OnAirborne(float elapsedTime);

        /// <summary>
        /// Calls when the projectile damages something
        /// </summary>
        /// <param name="damageEventArgs"></param>
        protected abstract void OnDamage(DamageEventArgs damageEventArgs);

        /// <summary>
        /// Calls when the projectile is in contact with something
        /// </summary>
        protected abstract void OnContact(int layer);

#if UNITY_EDITOR
        private void OnValidate() {
            hitBox = GetComponentInChildren<HitBoxEntityBase>();
        }
#endif
    }
}