using UnityEngine;
using XiheFramework.Combat.Damage;
using XiheFramework.Combat.Damage.HitBox;
using XiheFramework.Combat.Damage.Interfaces;
using XiheFramework.Core.Entity;
using XiheFramework.Core.LogicTime;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Projectile {
    public abstract class ProjectileEntity : GameEntity {
        public override string GroupName => "ProjectileEntity";
        public float lifeTime = 5f;

        public Vector3 StartPoint { get; private set; }
        public Vector3 EndPoint { get; private set; }

        protected float elapsedTime;
        private bool m_IsAirborne;

        public void Launch(Vector3 startPoint, Vector3 endPoint) {
            StartPoint = startPoint;
            EndPoint = endPoint;

            transform.position = StartPoint; //prevent it from spawning at vector3.zero and touch the ground 
            
            OnLaunch();

            m_IsAirborne = true;
            elapsedTime = 0f;
        }

        public void RestartLifeTime() {
            elapsedTime = 0f;
        }

        public void DestroyProjectile() {
            Game.Projectile.DestroyProjectile(EntityId);
        }

        public override void OnUpdateCallback() {
            if (!m_IsAirborne) {
                return;
            }

            elapsedTime += ScaledDeltaTime;
            OnAirborne();

            if (elapsedTime >= lifeTime) {
                DestroyProjectile();
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
        /// <param name="senderId"></param>
        /// <param name="receiverId"></param>
        /// <param name="hit"></param>
        protected abstract void OnDamageEnter(uint senderId, uint receiverId, RaycastHit hit);

        /// <summary>
        /// Calls when the projectile stay damages something
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="receiverId"></param>
        /// <param name="hit"></param>
        protected abstract void OnDamageStay(uint senderId, uint receiverId, RaycastHit hit);

        /// <summary>
        /// Calls when the projectile exits
        /// </summary>
        /// <param name="lastReceiverId"></param>
        protected abstract void OnDamageExit(uint lastReceiverId);
    }
}