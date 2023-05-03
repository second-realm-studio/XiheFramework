using System;
using UnityEngine;

namespace XiheFramework.Modules.Base {
    public abstract class GameModule : MonoBehaviour {
        protected virtual void Awake() {
            GameManager.RegisterComponent(this);
        }

        private void Update() {
            OnUpdate();
        }

        private void FixedUpdate() {
            OnFixedUpdate();
        }

        private void LateUpdate() {
            OnLateUpdate();
        }

        internal virtual void OnUpdate() { }

        internal virtual void OnFixedUpdate() { }

        internal virtual void OnLateUpdate() { }

        /// <summary>
        /// Called after all game modules are registered (End of Awake)
        /// Useful for setting up data before other modules trying to access it
        /// </summary>
        internal virtual void Setup() { }

        /// <summary>
        /// Callback function invoked when 
        /// </summary>
        /// <param name="shutDownType"></param>
        internal virtual void ShutDown(ShutDownType shutDownType) { }
    }
}