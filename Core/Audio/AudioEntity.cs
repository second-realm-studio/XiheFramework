using UnityEngine;
using XiheFramework.Core.Entity;
using XiheFramework.Core.LogicTime;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Audio {
    public class AudioEntity : GameEntity {
        public AudioClip AudioClip { get; set; }
        public bool Loop { get; set; }

        private AudioSource m_AudioSource;
        private bool m_Played;
        private float m_Timer;
        public override string GroupName => "AudioEntity";

        public void Play() {
            if (AudioClip) {
                m_AudioSource.clip = AudioClip;
                m_AudioSource.loop = Loop;
                m_AudioSource.Play();
            }

            m_Played = true;
        }

        public override void OnInitCallback() {
            base.OnInitCallback();

            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.playOnAwake = false;
            m_Played = false;
        }

        public override void OnUpdateCallback() {
            base.OnUpdateCallback();

            if (m_Played) {
                if (m_Timer >= AudioClip.length) {
                    if (!m_AudioSource.isPlaying) {
                        DestroyEntity();
                    }
                }

                m_Timer += ScaledDeltaTime;
            }
        }
    }
}