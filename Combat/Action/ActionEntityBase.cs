using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core.Entity;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Action {
    public abstract class ActionEntityBase : GameEntity {
        public override string GroupName => "ActionEntity";
        protected Dictionary<string, object> Arguments { get; private set; }
        
        public void ChangeAction(string actionAddress, params KeyValuePair<string, object>[] args) {
            Game.Action.ChangeAction(OwnerId, actionAddress, args);
        }

        public void SetArguments(params KeyValuePair<string, object>[] args) {
            if (Arguments == null) {
                Arguments = new Dictionary<string, object>();
            }
            else {
                Arguments.Clear();
            }

            foreach (var arg in args) {
                Arguments.Add(arg.Key, arg.Value);
            }
        }

        protected T FetchArgument<T>(string argumentName) {
            if (Arguments.TryGetValue(argumentName, out var argument)) {
                if (argument is T arg) {
                    return arg;
                }
            }

            if (Game.Action.enableDebug) {
                Debug.LogWarning("[Action] Action variable not found: " + argumentName);
            }

            return default;
        }
    }
}