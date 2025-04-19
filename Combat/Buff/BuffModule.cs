using UnityEngine;
using XiheFramework.Core.Base;
using XiheFramework.Core.Entity;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Buff {
    public class BuffModule : GameModule {
        public readonly string onBuffCreatedEvtName = "Buff.OnBuffCreated";
        public readonly string onBuffAddedEvtName = "Buff.OnBuffAdded";
        public readonly string onBuffRemovedEvtName = "Buff.OnBuffRemoved";
        public readonly string onBuffDestroyedEvtName = "Buff.OnBuffDestroyed";
        public readonly string onSetBuffValueEventName = "Buff.OnSetBuffValue";

        /// <summary>
        /// Add buff stack to owner IBuffOwner entity
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="buffAddress"></param>
        /// <param name="stack">0: max stack</param>
        public uint AddBuff(uint ownerId, string buffAddress, int stack = 1) {
            var owner = Game.Entity.GetEntity<GameEntity>(ownerId);
            if (owner == null) {
                return 0;
            }

            if (owner is not IBuffOwner buffOwner) {
                return 0;
            }

            // buff already exists
            BuffEntityBase buffEntity = null;
            if (buffOwner.HasBuff(buffAddress)) {
                if (stack == 0) {
                    stack = GetBuffEntity(ownerId, buffAddress).maxStack;
                }

                buffEntity = Game.Entity.GetEntity<BuffEntityBase>(buffOwner.GetBuffEntityId(buffAddress));
                var deltaStack = GetDeltaStack(buffEntity.CurrentStack, stack, buffEntity.maxStack);

                if (deltaStack == 0) {
                    if (enableDebug) Debug.Log($"[BUFF]MAX REACHED({buffEntity.maxStack}): {owner.name}({owner.EntityId})<-{buffAddress}({stack})");
                    return buffEntity.EntityId;
                }

                buffEntity.CurrentStack += deltaStack;
                buffEntity.OnBuffAddStack();
                var args = new OnAddBuffEventArgs(buffEntity.EntityId, buffAddress, deltaStack);
                Game.Event.InvokeNow(onBuffAddedEvtName, owner.EntityId, args);
            }
            else {
                buffEntity = Game.Entity.InstantiateEntity<BuffEntityBase>(buffAddress, owner.EntityId, true);
                if (stack == 0) {
                    stack = buffEntity.maxStack;
                }

                buffEntity.CurrentStack = stack;
                buffEntity.OnBuffEnter();
                var args = new OnBuffCreateEventArgs(buffEntity.EntityId, buffAddress);
                Game.Event.InvokeNow(onBuffCreatedEvtName, owner.EntityId, args);
            }

            if (buffEntity == null) return 0;

            if (enableDebug) {
                Debug.Log($"[BUFF]{owner.name}[{owner.EntityId}] gained {buffAddress}({stack})");
            }


            return buffEntity.EntityId;
        }

        /// <summary>
        /// Remove buff from owner CombatEntity
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="buffName"></param>
        /// <param name="stack">0: remove all</param>
        public void RemoveBuff(uint ownerId, string buffName, int stack = 1) {
            var owner = Game.Entity.GetEntity<GameEntity>(ownerId);
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
                    Debug.Log($"[BUFF]{owner.name}[{owner.EntityId}] lost {buffName}");
                }
            }
            else {
                buffEntity.CurrentStack -= stack;
                buffEntity.OnBuffRemoveStack();
                var eventArgs = new OnRemoveBuffEventArgs(buffEntity.EntityId, buffName, stack, buffEntity.CurrentStack - stack);
                Game.Event.InvokeNow(onBuffRemovedEvtName, owner.EntityId, eventArgs);
                if (enableDebug) {
                    Debug.Log($"[BUFF]{owner.name}[{owner.EntityId}] removed {stack}stacks of {buffName}");
                }
            }
        }

        public bool HasBuff(uint ownerId, string buffName) {
            var owner = Game.Entity.GetEntity<GameEntity>(ownerId);
            if (owner == null) {
                return false;
            }

            if (owner is not IBuffOwner buffOwner) {
                return false;
            }

            return buffOwner.HasBuff(buffName);
        }

        public BuffEntityBase GetBuffEntity(uint ownerId, string buffName) {
            var owner = Game.Entity.GetEntity<GameEntity>(ownerId);
            if (owner == null) {
                Debug.LogWarning($"[BUFF]Owner:{ownerId} doesn't exist");
                return null;
            }

            if (owner is not IBuffOwner buffOwner) {
                Debug.LogWarning($"[BUFF]Owner:{ownerId} is not IBuffOwner");
                return null;
            }

            if (buffOwner.HasBuff(buffName)) {
                return Game.Entity.GetEntity<BuffEntityBase>(buffOwner.GetBuffEntityId(buffName));
            }

            Debug.LogWarning($"[BUFF]{ownerId} doesn't have buff {buffName}");
            return null;
        }

        public void ClearBuff(uint ownerId) {
            var owner = Game.Entity.GetEntity<GameEntity>(ownerId);
            if (owner == null) {
                return;
            }

            if (owner is not IBuffOwner buffOwner) {
                return;
            }

            buffOwner.ClearBuff();
        }

        public void SetBuffValue(uint ownerId, string buffName, string valueName, object value) {
            var buffEntity = GetBuffEntity(ownerId, buffName);
            if (!buffEntity) {
                Debug.LogWarning($"[BUFF]{ownerId} doesn't have buff {buffName}");
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

        protected override void Awake() {
            base.Awake();
            Game.Buff = this;
        }
    }
}