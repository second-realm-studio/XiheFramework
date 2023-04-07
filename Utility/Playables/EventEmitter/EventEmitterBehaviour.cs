using System;
using UnityEngine.Playables;

namespace XiheFramework.Utility.Playables.EventEmitter {
    [Serializable]
    public class EventEmitterBehaviour : PlayableBehaviour {
        public string eventName;
        public string argument;
    }
}