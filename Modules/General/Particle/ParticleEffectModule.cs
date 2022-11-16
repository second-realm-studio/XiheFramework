using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class ParticleEffectModule : GameModule {
    public List<ParticleSystem> particlePrefabs = new List<ParticleSystem>();

    private Dictionary<string, ParticleSystem> m_ParticleCandidates = new Dictionary<string, ParticleSystem>();

    public Transform spawnRoot;

    public ParticleSystem SpawnParticle(string particleName, Vector3 worldPosition) {
        if (m_ParticleCandidates.ContainsKey(particleName)) {
            var particle = Instantiate(m_ParticleCandidates[particleName], spawnRoot);
            particle.transform.position = worldPosition;
            return particle;
        }

        return null;
    }

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

        foreach (var particlePrefab in particlePrefabs) {
            m_ParticleCandidates.Add(particlePrefab.gameObject.name, particlePrefab);
        }

        if (!spawnRoot) {
            var root = new GameObject("Root");
            spawnRoot = root.transform;
            spawnRoot.parent = transform;
        }
    }

    public override void Update() {
    }

    public override void ShutDown(ShutDownType shutDownType) {
        Destroy(spawnRoot);
        var root = new GameObject("Root");
        spawnRoot = root.transform;
        spawnRoot.parent = transform;
    }
}