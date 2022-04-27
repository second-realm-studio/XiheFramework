using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;


namespace XiheFramework.Rhythm {
    
}
public class PlayModule : GameModule {
    public AudioClip clip;
    public float volume;

    private AudioSource m_Source;

    private void Start() {
        GameObject go = new GameObject("Audio Source");
        m_Source = go.AddComponent<AudioSource>();
        m_Source.clip = clip;
        m_Source.playOnAwake = false;
        m_Source.volume = volume;
        m_Source.transform.SetParent(this.transform);
        
        Game.Event.Subscribe("OnPlay",OnPlay);
    }

    private void OnPlay(object sender, object e) {
        m_Source.Play();
    }

    public void Play() {
        m_Source.Play();
    }

    public void Pause() {
        m_Source.Pause();
    }

    public override void Update() {
        
    }

    public override void ShutDown(ShutDownType shutDownType) {
        
    }
}
