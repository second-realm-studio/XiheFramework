using UnityEngine;
using XiheFramework.Core.Audio.UnityAudio;

namespace XiheFramework.Core.Audio {
    public interface IAudioModule {
#if USE_WWISE
        void Play(AK.Wwise.Event audioEvent, GameObject container = null);
#else
        AudioPlayer Play(uint ownerId, AudioClip audioClip, Vector3 worldPosition, bool loop);
#endif
    }
}