//namespace XiheFramework.Core.Audio {
//    public class AudioModule: GameModule {
//        public void Play(string eventName, GameObject container=null){
//            AkSoundEngine.PostEvent(eventName , target);
//        }

//        public void Stop(uint eventId){
//          AkSoundEngine.StopPlayingID(eventId, 500, AkCurveInterpolation.AkCurveInterpolation_Constant);

//        }
      
//        public bool IsPlaying(uint eventId){
//          return AKSoundEngine.IsPlaying(eventId); //TODO: fix syntax
//        }

//        public void SetVariable(string variableName, object value, GameObject target){
//          AkSoundEngine.SetRTPCValue("GameParameter", value, target);
//        }

//        public void SetVariableGlobal(string variableName, object value){
//          AkSoundEngine.SetRTPCValue("GameParameter", value, AK_INVALID_GAME_OBJECT);
//        }

//        public void SetVariable(AK.Wwise.RTPC rtpc,object value, GameObject target){
//            rtpc.SetValue("GameParameter", value, gameObject);
//        }

//        public void SetVariableGlobal(AK.Wwise.RTPC rtpc,object value){
//            rtpc.SetGlobalValue("GameParameter", value);
//        }

//        public void TriggerSwitch(string switchName,string targetStateName, GameObject target){
//            AkSoundEngine.SetSwitch(string switchName, string targetStateName, GameObject target);
//        }

//        //TODO: add switch direct reference support
//        //public void TriggerSwitch(AK.Wwise.Switch switch,string targetStateName){
//        //    switch.Set
//        //}
//    }
//}
