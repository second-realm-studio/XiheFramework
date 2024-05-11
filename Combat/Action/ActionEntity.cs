using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Combat.Base;
using XiheFramework.Core;
using XiheFramework.Core.Entity;
using XiheFramework.Core.LogicTime;
using XiheFramework.Runtime;
using GeneralBlackboardNames = XiheFramework.Combat.Constants.GeneralBlackboardNames;

namespace XiheFramework.Combat.Action {
    public abstract class ActionEntity: TimeBasedGameEntity {
        
        private float m_TimeScale;

        protected Dictionary<string, object> Arguments { get; private set; }

        public override void OnInitCallback() {
            Arguments = new Dictionary<string, object>();
            OnActionInit();
            var owner = Game.Entity.GetEntity<CombatEntity>(OwnerId);
            Game.Blackboard.SetData(GeneralBlackboardNames.CombatEntity_CurrentActionName(owner), EntityAddressName);
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

        /// <summary>
        /// called one frame after OnActionExit(), useful to destroy any visual impact of the action, such as animation, particle, sound, etc.
        /// </summary>
        //protected abstract void OnActionUnload();
        public void ChangeAction(string actionName, params KeyValuePair<string, object>[] args) {
            Game.Action.ChangeAction(OwnerId, actionName, args);
        }

        public void SetArguments(params KeyValuePair<string, object>[] args) {
            Arguments.Clear();
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