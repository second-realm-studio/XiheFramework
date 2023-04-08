using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using XiheFramework.Modules.Base;

namespace XiheFramework.Modules.Audio {
    public class AudioModule : GameModule {
        [SerializeField] private Transform tempAudioRoot;
        [SerializeField] private List<ChannelInfo> typeChannelPairs = new();
        private readonly Dictionary<AudioChannelTypes, ChannelInfo> m_AudioMixerGroups = new();

        private readonly List<AudioSource> m_AudioSources = new();

        private void Start() {
            if (tempAudioRoot == null) Debug.LogError("temp audio root is null");

            if (typeChannelPairs.Count == 0) return;

            foreach (var pair in typeChannelPairs) m_AudioMixerGroups.Add(pair.type, pair);
        }
        
        /// <summary>
        /// Play Audio 
        /// </summary>
        /// <param name="audioSource"> target audio source, can be Null</param>
        /// <param name="channelType"></param>
        /// <param name="audioClip"></param>
        /// <param name="volume"> 0-1 </param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public AudioSource Play(AudioSource audioSource, AudioChannelTypes channelType, AudioClip audioClip, float volume,
            bool loop) {
            if (audioSource == null) {
                var go = new GameObject(audioClip.name);

                if (tempAudioRoot != null) {
                    Transform parent = null;
                    if (!tempAudioRoot.transform.Find(channelType.ToString())) {
                        parent = new GameObject(channelType.ToString()).transform;
                        parent.parent = tempAudioRoot;
                    }
                    else {
                        parent = tempAudioRoot.Find(channelType.ToString());
                    }

                    go.transform.parent = parent;
                }

                var src = go.AddComponent<AudioSource>();
                src.clip = audioClip;
                src.playOnAwake = false;
                src.loop = loop;
                src.volume = volume;
                src.outputAudioMixerGroup = m_AudioMixerGroups[channelType].channel;
                src.Play();

                m_AudioSources.Add(src);

                return src;
            }

            audioSource.clip = audioClip;
            audioSource.loop = loop;
            audioSource.volume = volume;
            audioSource.outputAudioMixerGroup = m_AudioMixerGroups[channelType].channel;
            audioSource.Play();

            if (!m_AudioSources.Contains(audioSource)) m_AudioSources.Add(audioSource);

            return audioSource;
        }
        
        public void SetPause(AudioSource audioSource, bool pause) {
            if (pause)
                audioSource.Pause();
            else
                audioSource.UnPause();
        }

        public void SetPauseAll(bool pause) {
            foreach (var audioSource in m_AudioSources) audioSource.Pause();
        }

        public void Stop(AudioSource audioSource) {
            audioSource.Stop();
        }

        /// <summary>
        /// Destroy all temporary AudioSource under the framework audio root
        /// </summary>
        public void StopAll() {
            var temp = tempAudioRoot.childCount;
            for (var i = 0; i < temp; i++) {
                var first = tempAudioRoot.GetChild(0);
                Destroy(first);
            }
        }

        internal override void ShutDown(ShutDownType shutDownType) {
            StopAll();
        }

        [Serializable]
        private struct ChannelInfo {
            public AudioChannelTypes type;
            public AudioMixerGroup channel;

            public ChannelInfo(AudioChannelTypes type, AudioMixerGroup channel, Transform spawnRoot) {
                this.type = type;
                this.channel = channel;
            }
        }
    }
}