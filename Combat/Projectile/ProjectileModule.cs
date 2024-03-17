using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Combat.Base;
using XiheFramework.Core.Base;

namespace XiheFramework.Combat.Projectile {
    public class ProjectileModule : GameModule {
        public int maxProjectileCount = 100;
        private Queue<ProjectileEntity> m_Projectiles = new();

        public ProjectileEntity InstantiateProjectile(CombatEntity owner, string projectileName) {
            var go = XiheFramework.Entry.Game.Resource.InstantiateAsset<GameObject>(ProjectileUtil.GetProjectileEntityAddress(projectileName));
            var entity = go.GetComponent<ProjectileEntity>();
            entity.SetOwnerId(owner);
            return entity;
        }

        public T InstantiateProjectile<T>(string projectileName) where T : ProjectileEntity {
            var go = XiheFramework.Entry.Game.Resource.InstantiateAsset<GameObject>(ProjectileUtil.GetProjectileEntityAddress(projectileName));
            var entity = go.GetComponent<ProjectileEntity>();
            return entity as T;
        }

        public void RegisterProjectile(ProjectileEntity projectile) {
            if (m_Projectiles.Count >= maxProjectileCount) {
                var oldProjectile = m_Projectiles.Dequeue();
                Destroy(oldProjectile.gameObject);
            }

            if (m_Projectiles.Contains(projectile)) {
                return;
            }

            m_Projectiles.Enqueue(projectile);
        }

        public void UnregisterProjectile(ProjectileEntity projectile) {
            var result = new List<ProjectileEntity>();
            while (m_Projectiles.Count > 0) {
                var p = m_Projectiles.Dequeue();
                if (p != projectile && p != null) {
                    result.Add(p);
                }
            }

            foreach (var p in result) {
                m_Projectiles.Enqueue(p);
            }
        }

        public ProjectileEntity[] GetProjectilesByOwner(CombatEntity owner) {
            var result = new List<ProjectileEntity>();
            foreach (var projectile in m_Projectiles) {
                if (projectile.Owner.EntityId == owner.EntityId) {
                    result.Add(projectile);
                }
            }

            return result.ToArray();
        }

        public T[] GetProjectilesByType<T>() where T : ProjectileEntity {
            var result = new List<T>();
            foreach (var projectile in m_Projectiles) {
                if (projectile is T tProjectile) {
                    result.Add(tProjectile);
                }
            }

            return result.ToArray();
        }

        public T[] GetProjectilesByOwnerAndType<T>(CombatEntity owner) where T : ProjectileEntity {
            var result = new List<T>();
            foreach (var projectile in m_Projectiles) {
                if (projectile.Owner.EntityId == owner.EntityId && projectile is T tProjectile) {
                    result.Add(tProjectile);
                }
            }

            return result.ToArray();
        }
    }
}