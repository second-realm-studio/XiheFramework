﻿using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework {
    public class UIModule : GameModule {
        private readonly Dictionary<string, UIBehaviour> m_UIBehaviours = new Dictionary<string, UIBehaviour>();

        //public Transform root;

        

        public void RegisterUIBehaviour(string behaviourName, UIBehaviour behaviour) {
            if (m_UIBehaviours.ContainsKey(behaviourName)) {
                m_UIBehaviours[behaviourName].UnActive();
                m_UIBehaviours.Remove(behaviourName);
            }

            m_UIBehaviours.Add(behaviourName, behaviour);
        }

        /// <summary>
        /// use UnActiveUI instead to hide ui
        /// </summary>
        /// <param name="behaviourName"></param>
        public void UnregisterUIBehaviour(string behaviourName) {
            if (m_UIBehaviours.ContainsKey(behaviourName)) {
                m_UIBehaviours.Remove(behaviourName);
            }
        }

        public bool ActivateUI(string behaviourName) {
            if (!m_UIBehaviours.ContainsKey(behaviourName)) {
                Debug.Log("[UI] target ui behaviour is not registered");
                return false;
            }

            m_UIBehaviours[behaviourName].Active();
            return true;
        }

        public bool UnactivateUI(string behaviourName) {
            if (!m_UIBehaviours.ContainsKey(behaviourName)) return false;

            m_UIBehaviours[behaviourName].UnActive();
            return true;
        }

        public bool TryGetUIBehaviour<T>(out T uiBehaviour) where T : UIBehaviour {
            var obj = FindObjectOfType<T>();
            if (obj) {
                uiBehaviour = obj;
                return obj;
            }

            uiBehaviour = null;
            //var go = Instantiate(uiBehaviour, gameObject.transform);
            return false;
        }

        public T InstantiateUIBehaviour<T>(T uiBehaviour) where T : UIBehaviour {
            return Instantiate(uiBehaviour);
        }

        public override void Update() { }
        public override void ShutDown(ShutDownType shutDownType) {
        }
        
    }
}