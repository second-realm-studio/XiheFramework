using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using XiheFramework.Core.Base;
using XiheFramework.Core.Entity;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Action {
    public class ActionModule : GameModule {
        public readonly string onChangeActionEventName = "Action.OnChangeAction";

        private Dictionary<uint, ChangeActionInfo> m_ChangingActions = new(); //prevent multiple action switch at the same frame. always favor last change call
        private Dictionary<uint, uint> m_CurrentActions = new();

        public void StopCurrentAction(uint ownerEntityId) {
            if (m_CurrentActions.ContainsKey(ownerEntityId)) {
                var currentActionId = m_CurrentActions[ownerEntityId];
                if (currentActionId != 0) {
                    Game.Entity.DestroyEntity(currentActionId);
                }

                m_CurrentActions[ownerEntityId] = 0;
            }
        }

        //TODO: change to last priority
        public void ChangeAction(uint ownerEntityId, string actionAddress, params KeyValuePair<string, object>[] args) {
            m_ChangingActions.TryAdd(ownerEntityId, new ChangeActionInfo(actionAddress, args));
        }

        public void SetCurrentActionArgument(uint ownerEntityId, string key, object value) {
            if (!m_CurrentActions.TryGetValue(ownerEntityId, out var actionId)) return;
            if (actionId != 0) {
                Game.Entity.GetEntity<ActionEntityBase>(actionId).SetArguments(new KeyValuePair<string, object>(key, value));
            }
        }

        protected override void Awake() {
            base.Awake();
            Game.Action = this;
        }

        public override void Setup() {
            Game.Event.Subscribe(Game.Entity.OnEntityDestroyedEvtName, OnEntityDestroyed);
        }

        public override void OnLateUpdate() {
            base.OnLateUpdate();

            if (m_ChangingActions == null || m_ChangingActions.Count == 0) return;

            //TODO: ToArray too expensive, fix it
            foreach (var changingAction in m_ChangingActions.ToArray()) {
                var ownerEntity = Game.Entity.GetEntity<GameEntity>(changingAction.Key);

                if (changingAction.Key == 0 || string.IsNullOrEmpty(changingAction.Value.actionAddress) || ownerEntity == null) {
                    continue;
                }

                var ownerId = ownerEntity.EntityId;

                //destroy current action
                if (m_CurrentActions.TryGetValue(ownerId, out var currentActionId)) {
                    if (currentActionId != 0) Game.Entity.DestroyEntity(currentActionId);
                    m_CurrentActions.Remove(ownerId);
                }

                //instantiate new action
                var actionAddress = changingAction.Value.actionAddress;
                var args = changingAction.Value.args;
                Game.Entity.InstantiateEntity<ActionEntityBase>(actionAddress, Vector3.zero, Quaternion.identity, ownerId, onInstantiatedCallback: entity => {
                    entity.SetArguments(args);
                    m_CurrentActions[ownerId] = entity.EntityId;

                    OnChangeActionArgs onChangeActionArgs = new OnChangeActionArgs(entity.EntityId, actionAddress, args);
                    Game.Event.Invoke(onChangeActionEventName, ownerId, onChangeActionArgs);
                    if (enableDebug) {
                        Debug.Log($"[Action] {ownerEntity.EntityAddress}({ownerId}) Change Action: {actionAddress}");
                    }
                });
            }

            m_ChangingActions.Clear();
        }

        private void OnEntityDestroyed(object sender, object e) {
            if (sender is not uint entityId) return;

            var args = (OnEntityDestroyedEventArgs)e;

            if (m_CurrentActions.ContainsKey(args.entityId)) {
                m_CurrentActions.Remove(args.entityId);
            }
        }

        private struct ChangeActionInfo {
            public string actionAddress;
            public KeyValuePair<string, object>[] args;

            public ChangeActionInfo(string actionAddress, KeyValuePair<string, object>[] args) {
                this.actionAddress = actionAddress;
                this.args = args;
            }
        }
    }
}