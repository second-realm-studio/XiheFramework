#if !USE_WWISE
using UnityEngine;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Audio.UnityAudio {
    public class UnityAudioModule : IAudioModule {
        public AudioPlayer Play(uint ownerId, AudioClip audioClip, Vector3 worldPosition, bool loop) {
            var audioPlayer = new GameObject("AudioPlayer(Dynamic)");
            var audioPlayerComponent = audioPlayer.AddComponent<AudioPlayer>();
            audioPlayer.transform.position = worldPosition;
            audioPlayerComponent.AudioClip = audioClip;
            audioPlayerComponent.Loop = loop;
            audioPlayerComponent.Play();

            return audioPlayerComponent;
        }
    }
}
#endif