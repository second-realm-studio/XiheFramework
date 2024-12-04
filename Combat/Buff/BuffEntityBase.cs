using System;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core.Entity;
using XiheFramework.Core.LogicTime;

namespace XiheFramework.Combat.Buff {
    public abstract class BuffEntityBase : GameEntity {
        public override string GroupName => "BuffEntity";

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

        private readonly Dictionary<string, object> m_BuffValues = new Dictionary<string, object>();

        public override void OnUpdateCallback() {
            OnBuffUpdate();
        }

        public override void OnDestroyCallback() {
            OnBuffDestroy();
        }

        public bool HasBuffValue(string valueName) {
            return m_BuffValues.ContainsKey(valueName);
        }

        public void SetBuffValue(string valueName, object value) {
            if (m_BuffValues.ContainsKey(valueName)) {
                m_BuffValues[valueName] = value;
            }
            else {
                m_BuffValues.Add(valueName, value);
            }

            OnBuffSetValue(valueName, value);
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

        public abstract void OnBuffAddStack();

        public abstract void OnBuffSetValue(string valueName, object value);

        public abstract void OnBuffUpdate();

        public abstract void OnBuffRemoveStack();

        public abstract void OnBuffDestroy();
    }
}