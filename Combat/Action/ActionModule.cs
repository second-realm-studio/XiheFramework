using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Combat.Base;
using XiheFramework.Core.Base;

namespace XiheFramework.Combat.Action {
    public class ActionModule : GameModule {
        public readonly string OnChangeActionEventName = "Action.OnChangeAction";

        private Dictionary<uint, bool> m_CombatEntitySwitchingStatus = new Dictionary<uint, bool>();

        internal override void Setup() {
            XiheFramework.Entry.Game.Event.Subscribe(OnChangeActionEventName, OnChangeAction);
        }

        private void OnChangeAction(object sender, object e) {
            var id = (uint)sender;
            if (m_CombatEntitySwitchingStatus.ContainsKey(id)) {
                m_CombatEntitySwitchingStatus[id] = false;
            }
        }

        public void ChangeAction(uint targetEntityId, string actionName, params KeyValuePair<string, object>[] args) {
            if (m_CombatEntitySwitchingStatus.ContainsKey(targetEntityId)) {
                if (m_CombatEntitySwitchingStatus[targetEntityId] == true) {
                    return;
                }

                m_CombatEntitySwitchingStatus[targetEntityId] = true;
            }
            else {
                m_CombatEntitySwitchingStatus.Add(targetEntityId, true);
            }

            OnChangeActionArgs onChangeActionArgs = new OnChangeActionArgs(actionName, args);
            XiheFramework.Entry.Game.Event.Invoke(OnChangeActionEventName, targetEntityId, onChangeActionArgs);

            if (enableDebug) {
                Debug.Log($"[Action] {XiheFramework.Entry.Game.Entity.GetEntity<CombatEntity>(targetEntityId).entityName}({targetEntityId}) Change Action: {actionName}");
            }
        }

        public ActionEntity LoadAction(string actionName) {
            var go = XiheFramework.Entry.Game.Resource.InstantiateAsset<GameObject>(ActionUtil.GetActionEntityAddress(actionName));
            if (go == null) {
                Debug.LogError($"{actionName} Action not found");
                return null;
            }

            var entity = go.GetComponent<ActionEntity>();
            if (entity == null) {
                Debug.LogError($"{actionName} Action prefab found, but ActionEntity script not found");
            }

            return entity;
        }
    }
}