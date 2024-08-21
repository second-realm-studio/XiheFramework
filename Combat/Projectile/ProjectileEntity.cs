using UnityEngine;
using XiheFramework.Combat.Damage;
using XiheFramework.Combat.Damage.HitBox;
using XiheFramework.Combat.Damage.Interfaces;
using XiheFramework.Core.LogicTime;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Projectile {
    public abstract class ProjectileEntity : TimeBasedGameEntity {
        public override string EntityGroupName => "ProjectileEntity";
        public HitBoxBase hitBox;
        public float lifeTime = 5f;

        public Vector3 StartPoint { get; private set; }
        public Vector3 EndPoint { get; private set; }

        protected float elapsedTime;

        private bool m_IsAirborne;

        public void Launch(Vector3 startPoint, Vector3 endPoint, System.Action<IDamageData> onDamageCallback) {
            StartPoint = startPoint;
            EndPoint = endPoint;

            hitBox.DisableHitBox();
            transform.position = StartPoint; //prevent it from spawning at vector3.zero and touch the ground 
            hitBox.OnHit = OnContact;
            hitBox.OnDamageDealt = OnDamage + onDamageCallback;
            hitBox.EnableHitBox(OwnerId);

            OnLaunch();

            m_IsAirborne = true;
            elapsedTime = 0f;
        }

        public void RestartLifeTime() {
            elapsedTime = 0f;
        }

        public override void OnUpdateCallback() {
            if (!m_IsAirborne) {
                return;
            }

            elapsedTime += ScaledDeltaTime;
            OnAirborne();

            if (elapsedTime >= lifeTime) {
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
        protected abstract void OnAirborne();

        /// <summary>
        /// Calls when the projectile damages something
        /// </summary>
        /// <param name="damageData"></param>
        protected abstract void OnDamage(IDamageData damageData);

        /// <summary>
        /// Calls when the projectile is in contact with something
        /// </summary>
        /// <param name="other"></param>
        protected abstract void OnContact(Collider other);

#if UNITY_EDITOR
        private void OnValidate() {
            hitBox = GetComponentInChildren<HitBoxBase>();
        }
#endif
    }
}