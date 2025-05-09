using XiheFramework.Core.Base;

namespace XiheFramework.Core.Audio.Wwise {
    public class WwiseAudioModule : GameModuleBase {
        public override int Priority => 0;
#if USE_WWISE
        private float m_MasterVolume = 50;

        public GameObject sharedAudioObject;
        private List<uint> m_PlayingEvents = new();
        
        public void Play(AK.Wwise.Event audioEvent, GameObject container = null) {
            if (container == null) {
                container = sharedAudioObject;
            }

            var id = audioEvent.Post(container);
            m_PlayingEvents.Add(id);
        }

        public void Play(AK.Wwise.Event audioEvent, GameObject container = null, AkCallbackManager.EventCallback callback = null) {
            if (container == null) {
                container = sharedAudioObject;
            }

            var id = audioEvent.Post(container, (uint)AkCallbackType.AK_EndOfEvent, callback);
            m_PlayingEvents.Add(id);
        }

        public void Stop(uint playingId, int fadeOutTime = 500, AkCurveInterpolation curveInterpolation = AkCurveInterpolation.AkCurveInterpolation_Linear) {
            AkSoundEngine.StopPlayingID(playingId, fadeOutTime, curveInterpolation);
        }

        public void SetMasterVolume(float volume) {
            volume = Mathf.Clamp(volume, 0, 100);
            m_MasterVolume = volume;
            AkSoundEngine.SetRTPCValue("MasterVolume", m_MasterVolume);
        }

        public void SetVariable(string variableName, float value, GameObject target) {
            AkSoundEngine.SetRTPCValue(variableName, value, target);
        }

        public void SetVariableGlobal(string variableName, float value, GameObject target) {
            AkSoundEngine.SetRTPCValue(variableName, value, target);
        }

        public void SetState(string stateGroupName, string state) {
            AkSoundEngine.SetState(stateGroupName, state);
        }
        
        public override void OnInstantiated() {
            base.OnInstantiated();
        }
        
        public override void OnDestroyed() {
            base.OnDestroyed();
            
            AkSoundEngine.StopAll();
        }
#endif
    }
}