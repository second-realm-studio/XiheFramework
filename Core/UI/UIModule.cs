using System;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Core.UI {
    public class UIModule : GameModule {
        private readonly Dictionary<string, UIBehaviour> m_UIBehaviours = new();

        public void RegisterUIBehaviour(string behaviourName, UIBehaviour behaviour) {
            if (m_UIBehaviours.ContainsKey(behaviourName)) {
                m_UIBehaviours[behaviourName].UnActive();
                m_UIBehaviours.Remove(behaviourName);
            }

            m_UIBehaviours.Add(behaviourName, behaviour);

            // if (behaviour.activeOnStart) {
            //     ActivateUI(behaviourName);
            // }
            // else {
            //     UnactivateUI(behaviourName);
            // }
        }

        /// <summary>
        ///     use UnActiveUI instead to hide ui
        /// </summary>
        /// <param name="behaviourName"></param>
        public void UnregisterUIBehaviour(string behaviourName) {
            if (m_UIBehaviours.ContainsKey(behaviourName)) m_UIBehaviours.Remove(behaviourName);
        }

        public bool ActivateUI(string behaviourName) {
            if (!m_UIBehaviours.ContainsKey(behaviourName)) {
                Debug.Log("[UI] " + behaviourName + " is not registered");
                return false;
            }

            m_UIBehaviours[behaviourName].gameObject.SetActive(true);
            m_UIBehaviours[behaviourName].Active();
            return true;
        }

        public bool UnactivateUI(string behaviourName) {
            if (!m_UIBehaviours.ContainsKey(behaviourName)) return false;
            if (m_UIBehaviours[behaviourName] == null) {
                m_UIBehaviours.Remove(behaviourName);
                return false;
            }

            m_UIBehaviours[behaviourName].UnActive();
            m_UIBehaviours[behaviourName].gameObject.SetActive(false);
            return true;
        }

        public bool TryGetUIBehaviour<T>(string uiName, out T uiBehaviour) where T : UIBehaviour {
            // var obj = FindObjectOfType<T>();
            // if (obj) {
            //     uiBehaviour = obj;
            //     return obj;
            // }
            //
            // uiBehaviour = null;
            // //var go = Instantiate(uiBehaviour, gameObject.transform);
            // return false;

            if (m_UIBehaviours.ContainsKey(uiName)) {
                uiBehaviour = m_UIBehaviours[uiName] as T;
                if (uiBehaviour != null) {
                    return true;
                }
            }

            uiBehaviour = null;
            return false;
        }

        /// <summary>
        /// Instantiate UI Behaviour, Important: This method is not allowed to be called in any callbacks, call Async version if in callback
        /// </summary>
        /// <param name="address"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T InstantiateUIBehaviour<T>(string address) where T : UIBehaviour {
            GameObject obj = Game.Resource.InstantiateAsset<GameObject>(address);
            T uiBehaviour = obj.GetComponent<T>();
            return uiBehaviour;
        }

        public UIBehaviour InstantiateUIBehaviour(string address) {
            return InstantiateUIBehaviour<UIBehaviour>(address);
        }

        public void InstantiateUIBehaviourAsync<T>(string address, Action<T> onInstantiated) where T : UIBehaviour {
            Game.Resource.InstantiateAssetAsync<GameObject>(address, (obj) => {
                T uiBehaviour = obj.GetComponent<T>();
                onInstantiated?.Invoke(uiBehaviour);
            });
        }

        public override void OnReset() {
            m_UIBehaviours.Clear();
        }

        protected override void Awake() {
            base.Awake();

            Game.UI = this;
        }
    }
}