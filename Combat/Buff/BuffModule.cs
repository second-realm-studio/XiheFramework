using System.Linq;
using UnityEngine;
using XiheFramework.Combat.Base;
using XiheFramework.Core;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Buff {
    public class BuffModule : GameModule {
        public readonly string onBuffCreatedEvtName = "Buff.OnBuffCreated";
        public readonly string onBuffAddedEvtName = "Buff.OnBuffAdded";
        public readonly string onBuffRemovedEvtName = "Buff.OnBuffRemoved";
        public readonly string onBuffDestroyedEvtName = "Buff.OnBuffDestroyed";
        public readonly string onSetBuffValueEventName = "Buff.OnSetBuffValue";

        /// <summary>
        /// Add buff stack to owner CombatEntity
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="buffName"></param>
        /// <param name="stack">0: max stack</param>
        public void AddBuff(uint ownerId, string buffName, int stack = 1) {
            var owner = Game.Entity.GetEntity<CombatEntity>(ownerId);
            if (owner == null) {
                return;
            }

            var currentBuffsArray = GetBuffEntityArray(ownerId);
            int buffIndex = -1;
            for (var i = 0; i < currentBuffsArray.Length; i++) {
                var entity = currentBuffsArray[i];
                if (entity.EntityAddressName == buffName) {
                    buffIndex = i;
                    break;
                }
            }

            // buff already exists
            if (buffIndex != -1) {
                if (stack == 0) {
                    stack = GetBuffEntity(ownerId, buffName).maxStack;
                }

                var buffEntity = currentBuffsArray[buffIndex];
                var deltaStack = GetDeltaStack(buffEntity.CurrentStack, stack, buffEntity.maxStack);

                if (deltaStack == 0) {
                    if (enableDebug) Debug.Log($"[BUFF]MAX REACHED({buffEntity.maxStack}): {owner.name}({owner.EntityId})<-{buffName}({stack})");
                    return;
                }

                buffEntity.CurrentStack += deltaStack;
                buffEntity.OnBuffAddStack();
                var args = new OnAddBuffEventArgs(buffEntity.EntityId, buffName, deltaStack);
                Game.Event.InvokeNow(onBuffAddedEvtName, owner.EntityId, args);
            }
            else {
                var buffAddress = BuffUtil.GetBuffEntityAddress(buffName);
                var buffEntity = Runtime.Game.Entity.InstantiateEntity<BuffEntity>(buffAddress, owner.EntityId, true);
                if (stack == 0) {
                    stack = buffEntity.maxStack;
                }

                buffEntity.CurrentStack = stack;
                buffEntity.OnBuffEnter();
                var args = new OnBuffCreateEventArgs(buffEntity.EntityId, buffName);
                Game.Event.InvokeNow(onBuffCreatedEvtName, owner.EntityId, args);
            }

            if (enableDebug) {
                Debug.Log($"[BUFF]{owner.EntityAddressName}[{owner.EntityId}] gained {buffName}({stack})");
            }
        }

        /// <summary>
        /// Remove buff from owner CombatEntity
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="buffName"></param>
        /// <param name="stack">0: remove all</param>
        public void RemoveBuff(uint ownerId, string buffName, int stack = 1) {
            var owner = Game.Entity.GetEntity<CombatEntity>(ownerId);
            if (owner == null) {
                return;
            }

            if (!GetBuffEntity(ownerId, buffName)) {
                Debug.LogWarning($"{owner.name}({owner.EntityId}) doesn't have buff {buffName}");
                return;
            }

            var buffEntity = GetBuffEntity(ownerId, buffName);

            if (stack == 0) {
                stack = buffEntity.CurrentStack;
            }

            if (buffEntity.CurrentStack <= stack) {
                Game.Entity.DestroyEntity(buffEntity.EntityId);
                var eventArgs = new OnBuffDestroyEventArgs(buffEntity.EntityId, buffName);
                Game.Event.InvokeNow(onBuffDestroyedEvtName, owner.EntityId, eventArgs);
                if (enableDebug) {
                    Debug.Log($"[BUFF]{owner.EntityAddressName}[{owner.EntityId}] lost {buffName}");
                }
            }
            else {
                buffEntity.CurrentStack -= stack;
                buffEntity.OnBuffRemoveStack();
                var eventArgs = new OnRemoveBuffEventArgs(buffEntity.EntityId, buffName, stack, buffEntity.CurrentStack - stack);
                Game.Event.InvokeNow(onBuffRemovedEvtName, owner.EntityId, eventArgs);
                if (enableDebug) {
                    Debug.Log($"[BUFF]{owner.EntityAddressName}[{owner.EntityId}] removed {stack}stacks of {buffName}");
                }
            }
        }

        public BuffEntity GetBuffEntity(uint ownerId, string buffName) {
            var owner = Runtime.Game.Entity.GetEntity<CombatEntity>(ownerId);
            var buffs = GetBuffEntityArray(ownerId);
            foreach (var buff in buffs) {
                if (buff.EntityAddressName == buffName) return buff;
            }

            return null;
        }

        public BuffEntity[] GetBuffEntityArray(uint ownerId) {
            var owner = Game.Entity.GetEntity<CombatEntity>(ownerId);
            var buffs = owner.BuffArray;
            var result = buffs.Select(buff => Game.Entity.GetEntity<BuffEntity>(buff)).ToArray();
            return result;
        }

        public void ClearBuff(uint ownerId) {
            foreach (var buffEntity in GetBuffEntityArray(ownerId)) {
                RemoveBuff(ownerId, buffEntity.EntityAddressName, 0);
            }
        }

        public void SetBuffValue(uint ownerId, string buffName, string valueName, object value) {
            var buffEntity = GetBuffEntity(ownerId, buffName);
            if (!buffEntity) {
                return;
            }

            buffEntity.SetBuffValue(valueName, value);

            var args = new OnSetBuffValueEventArgs(buffName, valueName, value);
            Game.Event.InvokeNow(onSetBuffValueEventName, ownerId, args);
        }

        public bool GetBuffValue<T>(uint ownerId, string buffName, string valueName, out T value) {
            var buff = GetBuffEntity(ownerId, buffName);
            if (!buff) {
                value = default;
                return false;
            }

            if (buff == null) {
                value = default;
                return false;
            }

            if (buff.GetBuffValue<T>(valueName, out var v)) {
                value = v;
                return true;
            }

            value = default;
            return false;
        }

        public int GetBuffStack(uint ownerId, string buffName) {
            if (!GetBuffEntity(ownerId, buffName)) {
                return 0;
            }

            var owner = Game.Entity.GetEntity<CombatEntity>(ownerId);
            return GetBuffEntity(ownerId, buffName).CurrentStack;
        }

        public void SetBuffMaxStack(uint ownerId, string buffName, int maxStack) {
            if (!GetBuffEntity(ownerId, buffName)) {
                return;
            }

            var buffEntity = GetBuffEntity(ownerId, buffName);
            buffEntity.maxStack = maxStack;
        }

        public int GetBuffMaxStack(uint ownerId, string buffName) {
            if (!GetBuffEntity(ownerId, buffName)) {
                return 0;
            }

            return GetBuffEntity(ownerId, buffName).maxStack;
        }

        private int GetDeltaStack(int currentStack, int addStack, int maxStack) {
            if (maxStack == 0) {
                return addStack;
            }

            return Mathf.Clamp(currentStack + addStack, 0, maxStack) - currentStack;
        }
    }
}