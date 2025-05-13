using XiheFramework.Core.Audio.UnityAudio;
using XiheFramework.Core.Audio.Wwise;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Audio {
    public abstract class AudioModuleBase : GameModuleBase {
        public override int Priority => (int)CoreModulePriority.Audio;

        public WwiseAudioModule Wwise {
            get {
                switch (this) {
                    case WwiseAudioModule wwise:
                        return wwise;
                    case UnityAudioModule:
                        Game.LogError("[AUDIO] Current Audio Module is a Unity Audio Module. Call Game.Audio.UnityAudio instead.");
                        break;
                    default:
                        Game.LogError($"[AUDIO] Current Audio Module is {this.GetType()}. ");
                        break;
                }

                return null;
            }
        }

        public UnityAudioModule UnityAudio {
            get {
                switch (this) {
                    case UnityAudioModule unityAudio:
                        return unityAudio;
                    case WwiseAudioModule:
                        Game.LogError("[AUDIO] Current Audio Module is a Wwise Audio Module. Call Game.Audio.Wwise instead.");
                        break;
                    default:
                        Game.LogError($"[AUDIO] Current Audio Module is {this.GetType()}. ");
                        break;
                }

                return null;
            }
        }

        public T CustomAudioModule<T>() where T : AudioModuleBase {
            if (this is not T audioModule) {
                Game.LogError($"[AUDIO] Current Audio Module is not a {typeof(T).Name}. Actual type is {GetType().Name}");
                return null;
            }

            return audioModule;
        }
    }
}