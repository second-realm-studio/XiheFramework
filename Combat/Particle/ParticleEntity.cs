using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Combat.Base;
using XiheFramework.Core.LogicTime;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Particle {
    public sealed class ParticleEntity : TimeBasedGameEntity {
        public string particleName;

        public ParticleSystem particle;
        private ParticleSystem.MainModule m_MainModule;
        private List<ParticleSystem.MainModule> m_SubMainModules = new List<ParticleSystem.MainModule>();
        private ParticleSystemRenderer m_ParticleSystemRenderer;
        private Transform m_CachedTransform;
        private bool m_Loop;

        public override string EntityGroupName => "ParticleEntity";
        public override string EntityAddressName => particleName;

        public override void OnInitCallback() {
            if (!particle) {
                particle = GetComponent<ParticleSystem>();
            }

            InitMainModules();
        }

#if UNITY_EDITOR
        private void OnValidate() {
            if (!particle) {
                particle = GetComponent<ParticleSystem>();
            }
        }
#endif

        protected override void OnSetGlobalTimeScale(object sender, object e) {
            base.OnSetGlobalTimeScale(sender, e);

            m_MainModule.simulationSpeed = timeScale;
            for (var i = 0; i < m_SubMainModules.Count; i++) {
                var subMainModule = m_SubMainModules[i];
                subMainModule.simulationSpeed = timeScale;
            }
        }

        /// <summary>
        /// Setup particle entity in local space
        /// </summary>
        /// <param name="localPosition"></param>
        /// <param name="localRotation"></param>
        /// <param name="localScale"></param>
        public void Setup(Vector3 localPosition, Quaternion localRotation, Vector3 localScale) {
            particle.Pause(true);
            var cachedTransform = transform;
            cachedTransform.localPosition = localPosition;
            cachedTransform.localRotation = localRotation;
            cachedTransform.localScale = localScale;
        }

        public void Play(bool loop) {
            m_MainModule.loop = loop;
            m_SubMainModules.ForEach(subMainModule => subMainModule.loop = loop);
            particle.Play(true);
        }
        
        public void SetMaterial(Material material) {
            m_ParticleSystemRenderer.material = material;
        }
        
        private void OnParticleSystemStopped() {
            Game.Particle.DestroyParticle(EntityId);
        }

        private void InitMainModules() {
            m_ParticleSystemRenderer = particle.GetComponent<ParticleSystemRenderer>();
            m_MainModule = particle.main;
            m_MainModule.playOnAwake = false;
            m_MainModule.stopAction = ParticleSystemStopAction.Callback;
            m_MainModule.simulationSpeed = timeScale;

            var childParticles = particle.GetComponentsInChildren<ParticleSystem>();
            for (var i = 1; i < childParticles.Length; i++) {
                var subParticle = childParticles[i];
                var subMainModule = subParticle.main;
                subMainModule.simulationSpeed = timeScale;
                m_SubMainModules.Add(subMainModule);
            }
        }
    }
}