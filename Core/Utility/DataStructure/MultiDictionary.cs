﻿using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace XiheFramework.Core.Utility.DataStructure {
    public class MultiDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, List<TValue>>> {
        private readonly Dictionary<TKey, List<TValue>> m_Dictionary;

        public MultiDictionary() {
            m_Dictionary = new Dictionary<TKey, List<TValue>>();
        }

        [ItemCanBeNull]
        public List<TValue> this[TKey key] => m_Dictionary[key];

        public Dictionary<TKey, List<TValue>>.KeyCollection Keys => m_Dictionary.Keys;
        public Dictionary<TKey, List<TValue>>.ValueCollection Values => m_Dictionary.Values;

        public int Count => m_Dictionary.Count;

        public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator() {
            return m_Dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(TKey key, TValue value) {
            if (key == null) {
                Debug.LogError("MultiDictionary key is null");
                return;
            }

            if (value == null) {
                Debug.LogError("MultiDictionary value is null");
                return;
            }

            if (m_Dictionary.ContainsKey(key)) {
                m_Dictionary[key].Add(value);
            }
            else {
                var list = new List<TValue> { value };
                m_Dictionary.Add(key, list);
            }
        }

        public void Remove(TKey key, TValue value) {
            if (key == null) {
                Debug.LogError("MultiDictionary key is null");
                return;
            }

            if (value == null) {
                Debug.LogError("MultiDictionary value is null");
                return;
            }

            if (!m_Dictionary.ContainsKey(key)) {
                Debug.LogErrorFormat("MultiDictionary Key {0} is not existed", key);
                return;
            }

            if (!m_Dictionary[key].Contains(value)) {
                Debug.LogErrorFormat("MultiDictionary Value {0} is not existed", value);
                return;
            }

            m_Dictionary[key].Remove(value);
            if (m_Dictionary[key].Count == 0) m_Dictionary.Remove(key);
        }

        public void RemoveList(TKey key) {
            if (key == null) {
                Debug.LogError("MultiDictionary key is null");
                return;
            }

            if (!m_Dictionary.ContainsKey(key)) {
                Debug.LogErrorFormat("MultiDictionary Key {0} is not existed", key);
                return;
            }

            m_Dictionary.Remove(key);
        }

        public List<TValue> GetList(TKey key) {
            if (key == null) {
                Debug.LogError("MultiDictionary key is null");
                return null;
            }

            return m_Dictionary[key];
        }

        public bool ContainsKey(TKey key) {
            if (key == null) {
                Debug.LogError("MultiDictionary key is null");
                return false;
            }

            return m_Dictionary.ContainsKey(key);
        }
        
        public bool TryGetValue(TKey key, out List<TValue> value) {
            if (key == null) {
                Debug.LogError("MultiDictionary key is null");
                value = null;
                return false;
            }

            return m_Dictionary.TryGetValue(key, out value);
        }

        public bool ContainsValue(TKey key, TValue value) {
            if (key == null) {
                Debug.LogError("MultiDictionary key is null");
                return false;
            }

            if (value == null) {
                Debug.LogError("MultiDictionary value is null");
                return false;
            }

            if (!m_Dictionary.ContainsKey(key)) return false;

            return m_Dictionary[key].Contains(value);
        }

        public void Clear() {
            m_Dictionary.Clear();
        }
    }
}