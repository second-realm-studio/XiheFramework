using System;
using System.Collections.Generic;

namespace XiheFramework.Combat.Action {
    [Serializable]
    public struct OnChangeActionArgs {
        public string actionName;
        public KeyValuePair<string, object>[] args;

        public OnChangeActionArgs(string actionName, KeyValuePair<string, object>[] args) {
            this.actionName = actionName;
            this.args = args;
        }
    }
}