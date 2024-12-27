using UnityEngine;

namespace XiheFramework.Core.Base {
    [DefaultExecutionOrder(-301)]
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

        public virtual void OnUpdate() { }

        public virtual void OnFixedUpdate() { }

        public virtual void OnLateUpdate() { }

        /// <summary>
        /// Called after all game modules are registered (End of Awake)
        /// Useful for setting up data before other modules trying to access it
        /// </summary>
        public virtual void Setup() { }

        public virtual void OnLateStart() { }

        public virtual void OnReset() { }

        public virtual void OnQuit() { }
    }
}