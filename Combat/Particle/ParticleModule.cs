using UnityEngine;
using XiheFramework.Core.Base;
using XiheFramework.Core.Entity;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Particle {
    public class ParticleModule : GameModule {
        public readonly string onParticleCreate = "Particle.OnParticleCreate";
        public readonly string onParticlePlay = "Particle.OnParticlePlay";
        public readonly string onParticleDestroy = "Particle.OnParticleDestroy";

        public ParticleEntity CreateParticleEntity(uint ownerId, string particleName, Vector3 localPosition, Quaternion localRotation, Vector3 localScale,
            bool followOwner = true) {
            var particleAddress = ParticleUtil.GetParticleEntityAddress(particleName);
            var particleEntity = Game.Entity.InstantiateEntity<ParticleEntity>(particleAddress, ownerId, followOwner, 0);
            localScale.Scale(particleEntity.gameObject.transform.localScale);
            particleEntity.Setup(localPosition, localRotation, localScale);
            Game.Event.Invoke(onParticleCreate, ownerId, particleEntity.EntityId);
            return particleEntity;
        }

        public void SetParticlePause(uint particleEntityId, bool pause) {
            var particleEntity = Game.Entity.GetEntity<ParticleEntity>(particleEntityId);
            if (particleEntity) {
                particleEntity.particle.Pause(pause);
            }
        }

        public ParticleEntity PlayParticle(uint ownerId, string particleName, bool loop = false, bool followOwner = false) {
            var ownerEntity = Game.Entity.GetEntity<GameEntity>(ownerId);
            if (ownerEntity != null) {
                return PlayParticle(ownerId, particleName, ownerEntity.transform.position, ownerEntity.transform.rotation, ownerEntity.transform.localScale, loop, followOwner);
            }

            return null;
        }

        public ParticleEntity PlayParticle(uint ownerId, string particleName, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, bool loop,
            bool followOwner = true) {
            var particle = CreateParticleEntity(ownerId, particleName, localPosition, localRotation, localScale, followOwner);
            particle.Play(loop);
            Game.Event.Invoke(onParticlePlay, ownerId, particle.EntityId);

            return particle;
        }

        public void DestroyParticle(uint particleEntityId) {
            Game.Entity.DestroyEntity(particleEntityId);
            Game.Event.Invoke(onParticleDestroy, particleEntityId);
        }

        protected override void Awake() {
            base.Awake();

            Game.Particle = this;
        }
    }
}