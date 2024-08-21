using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core.LogicTime;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Action {
    public abstract class ActionEntity : TimeBasedGameEntity {
        public override string EntityGroupName => "ActionEntity";
        protected Dictionary<string, object> Arguments { get; private set; }

        public override void OnInitCallback() {
            OnActionInit();
        }

        public override void OnUpdateCallback() {
            OnActionUpdate();
        }

        public override void OnDestroyCallback() {
            OnActionExit();
        }


        /// <summary>
        /// Setup data,events,child entity,etc. for the action. Should not be used to make any visual change in the scene
        /// </summary>
        protected abstract void OnActionInit();

        /// <summary>
        /// called on every frame(from the 2nd frame) when the action is updating
        /// </summary>
        protected abstract void OnActionUpdate();

        /// <summary>
        /// called at the same frame as the next action's OnActionInit, useful to clear data,events,etc.
        /// </summary>
        protected abstract void OnActionExit();

        public void ChangeAction(string actionName, params KeyValuePair<string, object>[] args) {
            Game.Action.ChangeAction(OwnerId, $"{EntityGroupName}_{actionName}", args);
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