using System;
using UnityEngine;
using XiheFramework.Core.Entity;
using XiheFramework.Runtime;

namespace XiheFramework.Core.UI {
    public abstract class UIBehaviour : GameEntity {
        public override string EntityGroupName => "UI";

        public string uiName;
        // public bool activeOnStart = false;

        private void Start() {
            Register();
            OnStart();
        }

        private void Register() {
            if (uiName == string.Empty) uiName = gameObject.name + gameObject.GetInstanceID();
            Game.UI.RegisterUIBehaviour(uiName, this);
        }

        protected abstract void OnStart();
        protected abstract void OnActive();
        protected abstract void OnUnActive();

        // protected abstract void OnDestroy();

        public void Active() {
            OnActive();
        }

        public void UnActive() {
            OnUnActive();
        }
    }
}