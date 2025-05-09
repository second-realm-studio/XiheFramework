using UnityEngine;
using XiheFramework.Core.Audio.UnityAudio;
using XiheFramework.Core.Base;

namespace XiheFramework.Core.Audio {
    public abstract class AudioModuleBase : GameModuleBase, IAudioModule {
        public override int Priority => 0;

        public abstract AudioPlayer Play(uint ownerId, AudioClip audioClip, Vector3 worldPosition, bool loop);
    }
}