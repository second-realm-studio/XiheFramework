using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Combat.Base;
using XiheFramework.Core;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Action {
    public class ActionModule : GameModule {
        public readonly string onChangeActionEventName = "Action.OnChangeAction";

        private Dictionary<uint, bool> m_CombatEntitySwitchingStatus = new Dictionary<uint, bool>();

        public void ChangeAction(uint ownerEntityId, string actionName, params KeyValuePair<string, object>[] args) {
            if (m_CombatEntitySwitchingStatus.ContainsKey(ownerEntityId)) {
                if (m_CombatEntitySwitchingStatus[ownerEntityId] == true) {
                    return;
                }

                m_CombatEntitySwitchingStatus[ownerEntityId] = true;
            }
            else {
                m_CombatEntitySwitchingStatus.Add(ownerEntityId, true);
            }

            var action = Game.Entity.InstantiateEntity<ActionEntity>(ActionUtil.GetActionEntityAddress(actionName));
            if (action == null) {
                Debug.LogError($"{actionName} Action not found");
                return;
            }

            action.OwnerId = ownerEntityId;
            action.SetArguments(args);

            OnChangeActionArgs onChangeActionArgs = new OnChangeActionArgs(action.EntityId, actionName, args);
            Game.Event.Invoke(onChangeActionEventName, ownerEntityId, onChangeActionArgs);

            if (enableDebug) {
                Debug.Log($"[Action] {Runtime.Game.Entity.GetEntity<CombatEntity>(ownerEntityId).EntityName}({ownerEntityId}) Change Action: {actionName}");
            }
        }

        public override void Setup() {
            Game.Event.Subscribe(onChangeActionEventName, OnChangeAction);
        }

        private void OnChangeAction(object sender, object e) {
            var id = (uint)sender;
            if (m_CombatEntitySwitchingStatus.ContainsKey(id)) {
                m_CombatEntitySwitchingStatus[id] = false;
            }
        }
    }
}