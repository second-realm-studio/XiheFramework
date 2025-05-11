using UnityEngine;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Audio.UnityAudio {
    public class UnityAudioModule : AudioModuleBase {
        public GameObject Play(AudioClip audioClip, GameObject follow = null, bool loop = false) {
            GameObject container;
            if (follow == null) {
                container = new GameObject("Unity Audio Player (Generated)");
                container.transform.position = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
            }
            else {
                container = follow;
            }

            var audioPlayerComponent = container.AddComponent<AudioPlayer>();
            audioPlayerComponent.AudioClip = audioClip;
            audioPlayerComponent.Play();

            return container;
        }
        
        
    }
}