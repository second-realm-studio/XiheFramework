using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Combat.Base;
using XiheFramework.Core;
using XiheFramework.Core.Base;
using XiheFramework.Core.Entity;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Action {
    public class ActionModule : GameModule {
        public readonly string onChangeActionEventName = "Action.OnChangeAction";

        private Dictionary<uint, bool> m_OwnerSwitchingActionStatus = new Dictionary<uint, bool>();//prevent multiple action switch at the same frame.

        private Dictionary<uint, uint> m_CurrentActions = new Dictionary<uint, uint>();

        public void ChangeAction(uint ownerEntityId, string actionName, params KeyValuePair<string, object>[] args) {
            if (m_OwnerSwitchingActionStatus.ContainsKey(ownerEntityId)) {
                if (m_OwnerSwitchingActionStatus[ownerEntityId] == true) {
                    return;
                }

                m_OwnerSwitchingActionStatus[ownerEntityId] = true;
            }
            else {
                m_OwnerSwitchingActionStatus.Add(ownerEntityId, true);
            }

            if (m_CurrentActions.ContainsKey(ownerEntityId)) {
                var currentActionId = m_CurrentActions[ownerEntityId];
                if (currentActionId != 0) {
                    Game.Entity.DestroyEntity(currentActionId);
                }

                m_CurrentActions[ownerEntityId] = 0;
            }
            else {
                m_CurrentActions.Add(ownerEntityId, 0);
            }

            var action = Game.Entity.InstantiateEntity<ActionEntity>(ActionUtil.GetActionEntityAddress(actionName));
            m_CurrentActions[ownerEntityId] = action.EntityId;

            if (action == null) {
                Debug.LogError($"{actionName} Action not found");
                return;
            }

            action.OwnerId = ownerEntityId;
            action.SetArguments(args);

            OnChangeActionArgs onChangeActionArgs = new OnChangeActionArgs(action.EntityId, actionName, args);
            Game.Event.Invoke(onChangeActionEventName, ownerEntityId, onChangeActionArgs);

            if (enableDebug) {
                Debug.Log($"[Action] {Runtime.Game.Entity.GetEntity<GameEntity>(ownerEntityId).EntityName}({ownerEntityId}) Change Action: {actionName}");
            }
        }

        public void SetCurrentActionArgument(uint ownerEntityId, string key, object value) {
            if (!m_CurrentActions.TryGetValue(ownerEntityId, out var actionId)) return;
            if (actionId != 0) {
                Game.Entity.GetEntity<ActionEntity>(actionId).SetArguments(new KeyValuePair<string, object>(key, value));
            }
        }

        public override void Setup() {
            Game.Event.Subscribe(onChangeActionEventName, OnChangeAction);
        }

        private void OnChangeAction(object sender, object e) {
            var id = (uint)sender;
            if (m_OwnerSwitchingActionStatus.ContainsKey(id)) {
                m_OwnerSwitchingActionStatus[id] = false;
            }
        }
    }
}