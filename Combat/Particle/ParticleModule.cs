using UnityEngine;
using XiheFramework.Combat.Base;
using XiheFramework.Core.Base;

namespace XiheFramework.Combat.Particle {
    public class ParticleModule : GameModule {
        public ParticleEntity Create(CombatEntity owner, string particleName, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, bool followOwner = true) {
            var particleAddress = ParticleUtil.GetParticleEntityAddress(particleName);
            var go = XiheFramework.Entry.Game.Resource.InstantiateAsset<GameObject>(particleAddress);
            var entity = go.GetComponent<ParticleEntity>();
            entity.Setup(owner, localPosition, localRotation, localScale, followOwner);
            return entity;
        }

        public ParticleEntity Play(CombatEntity owner, string particleName, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, bool loop,
            bool followOwner = true) {
            var particle = Create(owner, particleName, localPosition, localRotation, localScale, followOwner);
            particle.Play(loop);

            return particle;
        }

        public ParticleEntity Play(uint ownerId, string particleName, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, bool loop) {
            var owner = XiheFramework.Entry.Game.Entity.GetEntity<CombatEntity>(ownerId);
            if (!owner) return null;

            return Play(owner, particleName, localPosition, localRotation, localScale, loop);
        }

        public void DestroyParticle(ParticleEntity particle) {
            if (particle != null) {
                particle.StopAndDestroy();
            }
        }
    }
}