using System;
using UnityEngine;
using XiheFramework.Core.Event;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Base {
    public class XiheFrameworkEntry : MonoBehaviour {
        private void Awake() { }

        protected virtual void InstantiateCoreModules() {
            Game.InstantiateGameModuleAsync<EventModule>(null);
        }
    }
}