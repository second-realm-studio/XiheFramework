using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Combat.Base;
using XiheFramework.Core;
using GeneralBlackboardNames = XiheFramework.Combat.Constants.GeneralBlackboardNames;

namespace XiheFramework.Combat.Action {
    public abstract class ActionEntity : CombatEntityBase {
        private bool m_IsInitialized = false;
        private bool m_IsEnterPlayed = false;
        private bool m_IsGettingUnloaded = false;

        private float m_TimeScale;

        protected Dictionary<string, object> arguments { get; private set; }
        public CombatEntity OwnerCombatEntity { get; private set; }
        public CharacterController OwnerCharacterController => OwnerCombatEntity.CharacterController;

        public void Init(CombatEntity owner, params KeyValuePair<string, object>[] args) {
            m_IsInitialized = false;
            m_IsEnterPlayed = false;

            OwnerCombatEntity = owner;
            transform.parent = owner.actionRoot;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            var argsDict = new Dictionary<string, object>();
            foreach (var arg in args) {
                argsDict.Add(arg.Key, arg.Value);
            }

            arguments = argsDict;
            OnActionInit();

            m_IsInitialized = true;
            GameCore.Blackboard.SetData(GeneralBlackboardNames.CombatEntity_CurrentActionName(owner), entityName);
        }

        public void Exit() {
            m_IsGettingUnloaded = true;
            OnActionExit();
        }

        public void Unload() {
            OnActionUnload();
        }

        public void ChangeAction(string actionName, params KeyValuePair<string, object>[] args) {
            GameCombat.Action.ChangeAction(OwnerCombatEntity.GetEntityId(), actionName, args);
        }

        private void Update() {
            if (!m_IsInitialized) {
                return;
            }

            if (m_IsGettingUnloaded) {
                return;
            }

            if (!m_IsEnterPlayed) {
                OnActionEnter();
                m_IsEnterPlayed = true;
            }
            else {
                OnActionUpdate();
            }
        }

        /// <summary>
        /// Setup data,events,child entity,etc. for the action. Should not be used to make any visual change in the scene
        /// </summary>
        protected abstract void OnActionInit();

        /// <summary>
        /// called on the first frame when the action is updating, should be used to play animation, particle, sound, etc.
        /// </summary>
        protected abstract void OnActionEnter();

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
        protected abstract void OnActionUnload();

        protected T FetchArgument<T>(string argumentName) {
            if (arguments.ContainsKey(argumentName)) {
                if (arguments[argumentName] is T arg) {
                    return arg;
                }
            }

            Debug.LogWarning("Action variable not found: " + argumentName);
            return default;
        }
    }
}