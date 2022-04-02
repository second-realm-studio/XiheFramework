using UnityEngine;

namespace XiheFramework {
    public abstract class GameModule : MonoBehaviour {

        protected virtual void Awake() {
            GameManager.RegisterComponent(this);
        }

        public virtual void Setup() {
            
        }

        public abstract void Update();

        public abstract void ShutDown(ShutDownType shutDownType);
    }
}