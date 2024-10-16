using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Projectile {
    public class ProjectileModule : GameModule {
        public int maxProjectileCount = 100;

        public readonly string onProjectileInstantiatedEvtName = "Event.OnProjectileInstantiated";
        public readonly string onProjectileDestroyedEvtName = "Event.OnProjectileDestroyed";

        private int m_CurrentProjectileCount;

        public ProjectileEntity InstantiateProjectile(uint ownerId, string projectileName, bool followOwner = false) {
            var entity = InstantiateProjectile<ProjectileEntity>(ownerId, projectileName, followOwner);
            return entity;
        }

        public T InstantiateProjectile<T>(uint ownerId, string projectileName, bool followOwner = false) where T : ProjectileEntity {
            if (m_CurrentProjectileCount >= maxProjectileCount) {
                return null;
            }

            var entity = Game.Entity.InstantiateEntity<T>(ProjectileUtil.GetProjectileEntityAddress(projectileName), ownerId, followOwner);
            m_CurrentProjectileCount += 1;
            Game.Event.Invoke(onProjectileInstantiatedEvtName, entity.EntityId);
            return entity;
        }

        public void DestroyProjectile(uint projectileEntityId) {
            Game.Entity.DestroyEntity(projectileEntityId);
            m_CurrentProjectileCount -= 1;
            Game.Event.Invoke(onProjectileDestroyedEvtName, projectileEntityId);
        }

        // ↓↓↓ TODO: optimize!!

        // public ProjectileEntity[] GetProjectilesByOwner(uint ownerId) {
        //     var result = new List<ProjectileEntity>();
        //     foreach (var projectile in m_Projectiles) {
        //         if (projectile.OwnerId == ownerId) {
        //             result.Add(projectile);
        //         }
        //     }
        //
        //     return result.ToArray();
        // }
        //
        // public T[] GetProjectilesByType<T>() where T : ProjectileEntity {
        //     var result = new List<T>();
        //     foreach (var projectile in m_Projectiles) {
        //         if (projectile is T tProjectile) {
        //             result.Add(tProjectile);
        //         }
        //     }
        //
        //     return result.ToArray();
        // }
        //
        // public T[] GetProjectilesByOwnerAndType<T>(uint ownerId) where T : ProjectileEntity {
        //     var result = new List<T>();
        //     foreach (var projectile in m_Projectiles) {
        //         if (projectile.OwnerId == ownerId && projectile is T tProjectile) {
        //             result.Add(tProjectile);
        //         }
        //     }
        //
        //     return result.ToArray();
        // }

        protected override void Awake() {
            base.Awake();

            Game.Projectile = this;
        }
    }
}