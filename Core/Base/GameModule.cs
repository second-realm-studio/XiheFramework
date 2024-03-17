using UnityEngine;

namespace XiheFramework.Core.Base {
    public abstract class GameModule : MonoBehaviour {
        public int updateInterval;
        public int fixedUpdateInterval;
        public int lateUpdateInterval;

        public bool enableDebug;

        private int m_UpdateTimer;
        private int m_FixedUpdateTimer;
        private int m_LateUpdateTimer;

        protected virtual void Awake() {
            GameManager.RegisterComponent(this);
        }

        private void Update() {
            if (m_UpdateTimer >= updateInterval) {
                m_UpdateTimer -= updateInterval;
                OnUpdate();
            }
            else {
                m_UpdateTimer += 1;
            }
        }

        private void FixedUpdate() {
            if (m_FixedUpdateTimer >= fixedUpdateInterval) {
                m_FixedUpdateTimer -= fixedUpdateInterval;
                OnFixedUpdate();
            }
            else {
                m_FixedUpdateTimer += 1;
            }
        }

        private void LateUpdate() {
            if (m_LateUpdateTimer >= lateUpdateInterval) {
                m_LateUpdateTimer -= lateUpdateInterval;
                OnLateUpdate();
            }
            else {
                m_LateUpdateTimer += 1;
            }
        }

        internal virtual void OnUpdate() { }

        internal virtual void OnFixedUpdate() { }

        internal virtual void OnLateUpdate() { }

        /// <summary>
        /// Called after all game modules are registered (End of Awake)
        /// Useful for setting up data before other modules trying to access it
        /// </summary>
        internal virtual void Setup() { }

        internal virtual void OnLateStart() { }
        
        internal virtual void OnReset() { }
    }
}