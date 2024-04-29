using System;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Combat.Base;
using XiheFramework.Core;
using GeneralBlackboardNames = XiheFramework.Combat.Constants.GeneralBlackboardNames;

namespace XiheFramework.Combat.Buff {
    public abstract class BuffEntity : CombatEntityBase {
        public bool displayBuffIcon;

        /// <summary>
        /// 0=infinite
        /// </summary>
        public int maxStack;

        private int m_CurrentStack;

        public int CurrentStack {
            get => m_CurrentStack;
            set {
                if (maxStack == 0) {
                    m_CurrentStack = Mathf.Clamp(value, 0, int.MaxValue);
                }
                else {
                    m_CurrentStack = Mathf.Clamp(value, 0, maxStack);
                }
            }
        }

        public CombatEntity OwnerEntity { get; private set; }
        private readonly Dictionary<string, object> m_BuffValues = new Dictionary<string, object>();

        private bool m_Update = false;
        private string m_OnSetBuffValueEventHandlerId;

        public void OnBuffAdd(CombatEntity owner, int deltaStack) {
            if (OwnerEntity == null) {
                OwnerEntity = owner;
                transform.SetParent(owner.buffRoot, false);
                CurrentStack = deltaStack;
                OnBuffEnter();
            }
            else {
                CurrentStack += deltaStack;
            }

            GameCore.Blackboard.SetData(GeneralBlackboardNames.Buff_CurrentStack(OwnerEntity, this.entityName), CurrentStack);
            OnBuffAddStack();

            m_OnSetBuffValueEventHandlerId =GameCore.Event.Subscribe(GameCombat.Buff.OnSetBuffValueEventName, OnSetBuffValue);

            m_Update = true;
        }

        private void OnSetBuffValue(object sender, object e) {
            if (sender is not uint id || id != OwnerEntity.EntityId) {
                return;
            }

            var args = (OnSetBuffValueEventArgs)e;
            if (args.buffName != entityName) {
                return;
            }

            SetBuffValue(args.valueName, args.value);

            OnBuffSetValue(args.valueName, args.value);
        }

        private void Update() {
            if (m_Update) {
                OnBuffUpdate();
            }
        }

        public void SetBuffValue(string valueName, object value) {
            if (m_BuffValues.ContainsKey(valueName)) {
                m_BuffValues[valueName] = value;
            }
            else {
                m_BuffValues.Add(valueName, value);
            }
        }

        public bool GetBuffValue<T>(string valueName, out T value) {
            if (m_BuffValues.ContainsKey(valueName)) {
                var data = m_BuffValues[valueName];

                try {
                    value = (T)Convert.ChangeType(data, typeof(T));
                    return true;
                }
                catch (InvalidCastException) {
                    Debug.LogError($"[BUFF] dataName: {valueName} ({data.GetType().Name}]) can not cast to targetType: {typeof(T)}. InvalidCastException");
                    // not convertable
                }
                catch (FormatException) {
                    //format error
                    Debug.LogError($"[BUFF] dataName: {valueName} ({data.GetType().Name}]) can not cast to targetType: {typeof(T)}. FormatException");
                }
                catch (OverflowException) {
                    // value overflow
                    Debug.LogError($"[BUFF] dataName: {valueName} ({data.GetType().Name}]) can not cast to targetType: {typeof(T)}. OverflowException");
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Callback on buff added to combat entity for the first time
        /// </summary>
        public abstract void OnBuffEnter();

        /// <summary>
        /// 
        /// </summary>
        public abstract void OnBuffAddStack();

        public abstract void OnBuffSetValue(string valueName, object value);

        public abstract void OnBuffUpdate();

        public abstract void OnBuffRemoveStack();

        public abstract void OnBuffExit();

        public void OnBuffRemove(int stack) {
            CurrentStack -= stack;
            OnBuffRemoveStack();

            if (CurrentStack <= 0) {
                GameCore.Event.Unsubscribe(GameCombat.Buff.OnSetBuffValueEventName, m_OnSetBuffValueEventHandlerId);
                OnBuffExit();
            }
        }
    }
}