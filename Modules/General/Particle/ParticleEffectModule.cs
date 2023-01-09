using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XiheFramework;

public class ParticleEffectModule : GameModule {
    public List<NameParticlePair> particleBindings = new List<NameParticlePair>();

    private Dictionary<string, ParticleSystem> m_ParticleCandidates = new Dictionary<string, ParticleSystem>();

    public Transform spawnRoot;

    /// <summary>
    /// only spawn particle without playing it
    /// </summary>
    /// <param name="particleName"></param>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public ParticleSystem SpawnParticle(string particleName, Vector3 worldPosition) {
        if (m_ParticleCandidates.ContainsKey(particleName)) {
            var particle = Instantiate(m_ParticleCandidates[particleName], spawnRoot);
            particle.transform.position = worldPosition;
            return particle;
        }

        return null;
    }

    /// <summary>
    /// spawn and play particle system
    /// </summary>
    /// <param name="particleName"></param>
    /// <param name="worldPosition"></param>
    /// <param name="destroyAfterPlay"></param>
    /// <returns></returns>
    public ParticleSystem PlayParticle(string particleName, Vector3 worldPosition, bool destroyAfterPlay = true) {
        var particle = SpawnParticle(particleName, worldPosition);
        particle.Play(true);

        if (destroyAfterPlay) {
            Destroy(particle.gameObject, particle.main.duration);
        }

        return particle;
    }

    public override void Setup() {
        base.Setup();

        foreach (var pair in particleBindings) {
            m_ParticleCandidates.Add(pair.name, pair.particle);
        }

        if (!spawnRoot) {
            var root = new GameObject("Root");
            spawnRoot = root.transform;
            spawnRoot.parent = transform;
        }
    }

    public override void Update() { }

    public override void ShutDown(ShutDownType shutDownType) {
        Destroy(spawnRoot);
        var root = new GameObject("Root");
        spawnRoot = root.transform;
        spawnRoot.parent = transform;
    }

    [Serializable]
    public struct NameParticlePair {
        public string name;
        public ParticleSystem particle;

        public NameParticlePair(string name, ParticleSystem particle) {
            this.name = name;
            this.particle = particle;
        }
    }
}