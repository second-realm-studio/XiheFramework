using UnityEngine;

namespace XiheFramework {
    public abstract class GameModule : MonoBehaviour {

        protected virtual void Awake() {
            GameManager.RegisterComponent(this);
        }

        /// <summary>
        /// Called after all game modules are registered (End of Awake)
        /// Useful for setting up data before other modules trying to access it
        /// </summary>
        public virtual void Setup() {
            
        }

        public abstract void Update();

        public abstract void ShutDown(ShutDownType shutDownType);
    }
}