using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Combat.Base;

namespace XiheFramework.Combat.Particle {
    public class ParticleEntity : CombatEntityBase {
        public CombatEntity OwnerCombatEntity { get; private set; }

        public string particleName;

        public ParticleSystem particle;
        private ParticleSystem.MainModule m_MainModule;
        private List<ParticleSystem.MainModule> m_SubMainModules = new List<ParticleSystem.MainModule>();
        private ParticleSystemRenderer m_ParticleSystemRenderer;
        private Transform m_CachedTransform;
        private System.Action m_OnStopCallback;
        private bool m_AllowPlay;
        private bool m_Loop;

        public override string entityName => particleName;

        private void Awake() {
            if (!particle) {
                particle = GetComponent<ParticleSystem>();
            }
        }

        protected override void Start() {
            base.Start();

            InitMainModules();
            if (m_AllowPlay) {
                OnPlay();
                m_AllowPlay = false;
            }
        }

#if UNITY_EDITOR
        private void OnValidate() {
            if (!particle) {
                particle = GetComponent<ParticleSystem>();
            }
        }
#endif

        private void Update() {
            if (m_AllowPlay) {
                OnPlay();
                m_AllowPlay = false;
            }
        }

        protected override void OnSetGlobalTimeScale(object sender, object e) {
            base.OnSetGlobalTimeScale(sender, e);

            m_MainModule.simulationSpeed = timeScale;
            for (var i = 0; i < m_SubMainModules.Count; i++) {
                var subMainModule = m_SubMainModules[i];
                subMainModule.simulationSpeed = timeScale;
            }
        }


        public void Setup(CombatEntity owner, Vector3 position, Quaternion rotation, Vector3 scale, bool followOwner) {
            if (owner == null) {
                StopAndDestroy();
            }

            particle.Pause(true);

            OwnerCombatEntity = owner;
            m_CachedTransform = transform;
            if (followOwner) {
                m_CachedTransform.SetParent(owner.particleRoot, false);
                m_CachedTransform.localPosition = position;
                m_CachedTransform.localRotation = rotation;
                m_CachedTransform.localScale = scale;
            }
            else {
                m_CachedTransform.position = owner.transform.TransformPoint(position);
                m_CachedTransform.rotation = rotation;
                m_CachedTransform.localScale = scale;
            }
        }

        public void Play(bool loop) {
            m_Loop = loop;
            m_AllowPlay = true;
        }

        private void OnPlay() {
            m_MainModule.loop = m_Loop;
            m_SubMainModules.ForEach(subMainModule => subMainModule.loop = m_Loop);
            particle.Play(true);
        }

        public void StopAndDestroy() {
            Destroy(gameObject);
        }

        public void SetOnStopCallback(System.Action onStop) {
            m_OnStopCallback = onStop;
        }

        public void SetMaterial(Material material) {
            m_ParticleSystemRenderer.material = material;
        }

        private void OnParticleSystemStopped() {
            StopAndDestroy();
            m_OnStopCallback?.Invoke();
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