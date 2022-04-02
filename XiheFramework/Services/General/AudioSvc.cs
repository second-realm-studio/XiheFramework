using UnityEngine;

namespace XiheFramework {
    public static class AudioSvc {
        public static AudioSource PlayAudio(AudioSource audioSource, AudioChannelTypes channelType, AudioClip audioClip, float volume, bool loop) {
            return Game.Audio.Play(audioSource, channelType, audioClip, volume, loop);
        }

        public static void PauseAudio(AudioSource audioSource) {
            Game.Audio.SetPause(audioSource, true);
        }

        public static void UnPauseAudio(AudioSource audioSource) {
            Game.Audio.SetPause(audioSource, false);
        }

        public static void StopAudio(AudioSource audioSource) {
            Game.Audio.Stop(audioSource);
        }
    }
}