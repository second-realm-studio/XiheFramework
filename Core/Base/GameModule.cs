using System;
using UnityEngine;

namespace XiheFramework.Core.Base {
    public abstract class GameModuleBase : MonoBehaviour {
        /// <summary>
        /// Priority of the module to determine the order of execution
        /// The smaller the number, the earlier it will be executed
        /// Default value is 0
        /// </summary>
        public abstract int Priority { get; }

        public int updateInterval;
        public int fixedUpdateInterval;
        public int lateUpdateInterval;

        public bool enableDebug;

        #region Game Module Callbacks

        internal void OnInstantiatedInternal(Action onInstantiated) {
            OnInstantiated();
            onInstantiated?.Invoke();
        }

        internal void OnUpdateInternal() => OnUpdate();

        internal void OnFixedUpdateInternal() => OnFixedUpdate();

        internal void OnLateUpdateInternal() => OnLateUpdate();

        internal void OnDestroyedInternal() => OnDestroyed();

        /// <summary>
        /// Called after all modules that got instantiated at that frame are instantiated
        /// </summary>
        protected virtual void OnInstantiated() { }

        protected virtual void OnUpdate() { }
        protected virtual void OnFixedUpdate() { }
        protected virtual void OnLateUpdate() { }
        protected virtual void OnDestroyed() { }

        #endregion
    }
}