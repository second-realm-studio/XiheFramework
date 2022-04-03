using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace XiheFramework.Util {
    public class MultiDictionary<TKey, TValue>:IEnumerable<KeyValuePair<TKey,List<TValue>>> {
        private readonly Dictionary<TKey, List<TValue>> m_Dictionary;

        [ItemCanBeNull] public List<TValue> this[TKey key] => m_Dictionary[key];

        public Dictionary<TKey, List<TValue>>.KeyCollection Keys => m_Dictionary.Keys;
        public Dictionary<TKey, List<TValue>>.ValueCollection Values => m_Dictionary.Values;

        public MultiDictionary() {
            m_Dictionary = new Dictionary<TKey, List<TValue>>();
        }

        public void Add(TKey key, TValue value) {
            if (key == null) {
                Game.Log.LogError("MultiDictionary key is null");
                return;
            }

            if (value == null) {
                Game.Log.LogError("MultiDictionary value is null");
                return;
            }

            if (m_Dictionary.ContainsKey(key)) {
                m_Dictionary[key].Add(value);
            }
            else {
                var list = new List<TValue> {value};
                m_Dictionary.Add(key, list);
            }
        }

        public void Remove(TKey key, TValue value) {
            if (key == null) {
                Game.Log.LogError("MultiDictionary key is null");
                return;
            }

            if (value == null) {
                Game.Log.LogError("MultiDictionary value is null");
                return;
            }

            if (!m_Dictionary.ContainsKey(key)) {
                Game.Log.LogErrorFormat("MultiDictionary Key {0} is not existed", key);
                return;
            }

            if (!m_Dictionary[key].Contains(value)) {
                Game.Log.LogErrorFormat("MultiDictionary Value {0} is not existed", value);
                return;
            }

            m_Dictionary[key].Remove(value);
            if (m_Dictionary[key].Count == 0) {
                m_Dictionary.Remove(key);
            }
        }

        public List<TValue> GetList(TKey key) {
            if (key == null) {
                Game.Log.LogError("MultiDictionary key is null");
                return null;
            }

            return m_Dictionary[key];
        }

        public bool ContainsKey(TKey key) {
            if (key == null) {
                Game.Log.LogError("MultiDictionary key is null");
                return false;
            }

            return m_Dictionary.ContainsKey(key);
        }

        public bool ContainsValue(TKey key, TValue value) {
            if (key == null) {
                Game.Log.LogError("MultiDictionary key is null");
                return false;
            }

            if (value == null) {
                Game.Log.LogError("MultiDictionary value is null");
                return false;
            }

            return m_Dictionary[key].Contains(value);
        }

        public void Clear() {
            m_Dictionary.Clear();
        }

        public int Count => m_Dictionary.Count;
        
        public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator() {
            return m_Dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

    }
}