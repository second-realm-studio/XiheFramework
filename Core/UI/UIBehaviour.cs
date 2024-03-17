using UnityEngine;
using XiheFramework.Entry;

namespace XiheFramework.Core.UI {
    public abstract class UIBehaviour : MonoBehaviour {
        public string uiName;
        public bool activeOnStart = false;

        private void Start() {
            Register();
            OnStart();
        }

        private void Register() {
            if (uiName == string.Empty) uiName = gameObject.name + gameObject.GetInstanceID();
            Game.UI.RegisterUIBehaviour(uiName, this);

            if (activeOnStart) {
                Active();
            }
            else {
                UnActive();
            }
        }

        protected virtual void OnStart() { }

        protected virtual void OnActive() { }

        protected virtual void OnUnActive() { }

        public void Active() {
            gameObject.SetActive(true);
            OnActive();
        }

        public void UnActive() {
            gameObject.SetActive(false);
            OnUnActive();
        }
    }
}