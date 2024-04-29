using System.Linq;
using UnityEngine;
using XiheFramework.Combat.Base;
using XiheFramework.Core;
using XiheFramework.Core.Base;

namespace XiheFramework.Combat.Buff {
    public class BuffModule : GameModule {
        public readonly string OnAddBuffEventName = "Buff.OnAddBuff";
        public readonly string OnRemoveBuffEventName = "Buff.OnRemoveBuff";
        public readonly string OnSetBuffValueEventName = "Buff.OnSetBuffValue";

        public void AddBuff(uint ownerId, string buffName, int stack = 1) {
            var owner = GameCore.Entity.GetEntity<CombatEntity>(ownerId);
            if (owner == null) {
                Debug.LogError($"[BUFF]Owner not found: {ownerId}");
                return;
            }

            AddBuff(owner, buffName, stack);
        }

        public void AddBuff(CombatEntity owner, string buffName, int stack = 1) {
            if (stack < 1) {
                return;
            }

            var currentBuffsArray = owner.BuffArray;
            OnAddBuffEventArgs args;
            if (currentBuffsArray.Contains(buffName)) {
                var buffEntity = owner.GetBuffEntity(buffName);
                var deltaStack = GetDeltaStack(buffEntity.CurrentStack, stack, buffEntity.maxStack);

                if (deltaStack == 0) {
                    if (enableDebug) {
                        Debug.LogWarning($"[BUFF]MAX REACHED({buffEntity.maxStack}): {owner.name}({owner.GetEntityId()})<-{buffName}({stack})");
                    }

                    return;
                }

                args = new OnAddBuffEventArgs(buffName, deltaStack, null);
            }
            else {
                var buffAddress = BuffUtil.GetBuffEntityAddress(buffName);
                var go = GameCore.Resource.InstantiateAsset<GameObject>(buffAddress);
                if (go == null) {
                    Debug.LogError("[BUFF] BuffEntity not found: " + buffName);
                    return;
                }

                var buff = go.GetComponent<BuffEntity>();
                var deltaStack = GetDeltaStack(0, stack, buff.maxStack);
                args = new OnAddBuffEventArgs(buffName, deltaStack, buff);
            }

            if (enableDebug) {
                Debug.Log($"[BUFF]{owner.entityName}[{owner.EntityId}] gained {buffName}({stack})");
            }

            GameCore.Event.InvokeNow(OnAddBuffEventName, owner.GetEntityId(), args);
        }

        private int GetDeltaStack(int currentStack, int addStack, int maxStack) {
            if (maxStack == 0) {
                return addStack;
            }

            return Mathf.Clamp(currentStack + addStack, 0, maxStack) - currentStack;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="buffName"></param>
        /// <param name="stack">0: remove all</param>
        public void RemoveBuff(uint ownerId, string buffName, int stack = 1) {
            var owner = GameCore.Entity.GetEntity<CombatEntity>(ownerId);
            RemoveBuff(owner, buffName, stack);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="buffName"></param>
        /// <param name="stack">0: remove all</param>
        public void RemoveBuff(CombatEntity owner, string buffName, int stack = 1) {
            if (!owner.HasBuff(buffName)) {
                // Debug.LogWarning($"{owner.name}({owner.GetEntityId()}) doesn't have buff {buffName}");
                return;
            }

            var eventArgs = new OnRemoveBuffEventArgs(buffName, stack == 0 ? owner.GetBuffEntity(buffName).CurrentStack : stack);

            if (enableDebug) {
                Debug.Log($"[BUFF]{owner.entityName}[{owner.EntityId}] lost {buffName}({stack})");
            }

            GameCore.Event.InvokeNow(OnRemoveBuffEventName, owner.GetEntityId(), eventArgs);
        }

        public void ClearBuff(CombatEntity owner) {
            foreach (var buffName in owner.BuffArray) {
                RemoveBuff(owner.EntityId, buffName, 0);
            }
        }

        public void SetBuffValue(uint ownerId, string buffName, string valueName, object value) {
            var owner = GameCore.Entity.GetEntity<CombatEntity>(ownerId);
            SetBuffValue(owner, buffName, valueName, value);
        }

        public void SetBuffValue(CombatEntity owner, string buffName, string valueName, object value) {
            var hasBuff = owner.HasBuff(buffName);
            if (!hasBuff) {
                return;
            }

            var args = new OnSetBuffValueEventArgs(buffName, valueName, value);
            GameCore.Event.InvokeNow(OnSetBuffValueEventName, owner.GetEntityId(), args);
        }

        public bool GetBuffValue<T>(uint ownerId, string buffName, string valueName, out T value) {
            var owner = GameCore.Entity.GetEntity<CombatEntity>(ownerId);
            return GetBuffValue(owner, buffName, valueName, out value);
        }

        public bool GetBuffValue<T>(CombatEntity owner, string buffName, string valueName, out T value) {
            var hasBuff = owner.HasBuff(buffName);
            if (!hasBuff) {
                value = default;
                return false;
            }

            var buffEntity = owner.GetBuffEntity(buffName);
            if (buffEntity == null) {
                value = default;
                return false;
            }

            if (buffEntity.GetBuffValue<T>(valueName, out var v)) {
                value = v;
                return true;
            }

            value = default;
            return false;
        }

        public bool HasBuff(uint ownerId, string buffName) {
            var owner = GameCore.Entity.GetEntity<CombatEntity>(ownerId);
            return owner.HasBuff(buffName);
        }

        public int GetBuffStack(uint ownerId, string buffName) {
            if (!HasBuff(ownerId, buffName)) {
                return 0;
            }

            var owner = GameCore.Entity.GetEntity<CombatEntity>(ownerId);
            return owner.GetBuffEntity(buffName).CurrentStack;
        }

        public void SetBuffMaxStack(uint ownerId, string buffName, int maxStack) {
            if (!HasBuff(ownerId, buffName)) {
                return;
            }

            var owner = GameCore.Entity.GetEntity<CombatEntity>(ownerId);
            var buffEntity = owner.GetBuffEntity(buffName);
            buffEntity.maxStack = maxStack;
        }

        public int GetBuffMaxStack(uint ownerId, string buffName) {
            if (!HasBuff(ownerId, buffName)) {
                return 0;
            }

            var owner = GameCore.Entity.GetEntity<CombatEntity>(ownerId);
            return owner.GetBuffEntity(buffName).maxStack;
        }
    }
}