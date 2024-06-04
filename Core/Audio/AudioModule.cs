using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core.Base;

namespace XiheFramework.Core.Audio {
    public class AudioModule : GameModule {
        private List<uint> m_PlayingEvents = new();

#if USE_WWISE
        public void Play(AK.Wwise.Event audioEvent, GameObject container = null, AkCallbackManager.EventCallback callback = null) {
            var id = audioEvent.Post(container, (uint)AkCallbackType.AK_EndOfEvent, callback);
            m_PlayingEvents.Add(id);
        }

        public void Stop(uint playingId, int fadeOutTime = 500, AkCurveInterpolation curveInterpolation = AkCurveInterpolation.AkCurveInterpolation_Linear) {
            AkSoundEngine.StopPlayingID(playingId, fadeOutTime, curveInterpolation);
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

#endif

        //  public void SetVariable(AK.Wwise.RTPC rtpc,object value, GameObject target){
        //      rtpc.SetValue("GameParameter", value, gameObject);
        //  }
        //
        //  public void SetVariableGlobal(AK.Wwise.RTPC rtpc,object value){
        //      rtpc.SetGlobalValue("GameParameter", value);
        //  }
        //
        //  public void TriggerSwitch(string switchName,string targetStateName, GameObject target){
        //      AkSoundEngine.SetSwitch(string switchName, string targetStateName, GameObject target);
        //  }
        //
        // // TODO: add switch direct reference support
        // public void TriggerSwitch(AK.Wwise.Switch switch,string targetStateName){
        //     switch.Set
        // }
    }
}