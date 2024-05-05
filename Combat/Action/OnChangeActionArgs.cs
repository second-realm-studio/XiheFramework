using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace XiheFramework.Combat.Action {
    [Serializable]
    public struct OnChangeActionArgs {
        public uint actionEntityId;
        public string actionEntityName;
        public KeyValuePair<string, object>[] args;

        public OnChangeActionArgs(uint actionEntityId, string actionEntityName, KeyValuePair<string, object>[] args) {
            this.actionEntityId = actionEntityId;
            this.actionEntityName = actionEntityName;
            this.args = args;
        }
    }
}