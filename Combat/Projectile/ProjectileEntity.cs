using UnityEngine;
using XiheFramework.Combat.Base;
using XiheFramework.Combat.Damage.DataTypes;
using XiheFramework.Combat.Damage.Hitbox;
using XiheFramework.Entry;

namespace XiheFramework.Combat.Projectile {
    public abstract class ProjectileEntity : CombatEntityBase {
        public CombatEntity Owner { get; private set; }
        public HitBoxBase hitbox;
        public float lifeTime = 5f;

        public Vector3 StartPoint { get; private set; }
        public Vector3 EndPoint { get; private set; }

        private bool m_IsAirborne;
        private float m_LifeTimer;

        public void SetOwnerId(CombatEntity owner) {
            Owner = owner;
            hitbox.SetOwnerId(owner);
        }

        public void Launch(Vector3 startPoint, Vector3 endPoint, System.Action<DamageEventArgs> onDamageCallback) {
            if (this == null) {
                DestroyBullet();
                return;
            }

            Game.Projectile.RegisterProjectile(this);

            System.Action<int> action = _ => { m_IsAirborne = false; };
            action += OnContact;
            hitbox.SetHitCallback(action);

            StartPoint = startPoint;
            EndPoint = endPoint;

            hitbox.DeactivateDamageSender();
            onDamageCallback += OnDamage;
            hitbox.SetDamageCallback(onDamageCallback);
            hitbox.ActivateDamageSender();

            OnLaunch();

            m_IsAirborne = true;
            m_LifeTimer = 0f;
        }

        public void Update() {
            if (!m_IsAirborne) {
                return;
            }

            m_LifeTimer += scaledDeltaTime;
            OnAirborne(m_LifeTimer);

            if (m_LifeTimer >= lifeTime) {
                DestroyBullet();
            }
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

        public void DestroyBullet(float delay = 0f) {
            Game.Projectile.UnregisterProjectile(this);
            StopAllCoroutines();
            Destroy(gameObject, delay);
        }

#if UNITY_EDITOR
        private void OnValidate() {
            hitbox = GetComponentInChildren<HitBoxBase>();
        }
#endif
    }
}