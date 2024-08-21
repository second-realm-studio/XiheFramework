using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using XiheFramework.Combat.Damage.Interfaces;
using XiheFramework.Core;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Damage {
    public abstract class DamageModuleBase : GameModule, IDamageModule {
        public string onProcessedDamageEventName => "Damage.OnProcessedDamage";
        protected Queue<IDamageData> damageQueue = new();

        public void RegisterDamage(IDamageData damageData) {
            damageQueue.Enqueue(damageData);
        }

        protected override void Awake() {
            base.Awake();

            Game.Damage = this;
        }

        public override void OnLateUpdate() {
            //TODO: (Maybe) change to concurrent queue
            if (damageQueue.Count > 0) {
                IDamageData damageData = damageQueue.Dequeue();
                var valid = Process(damageData, out var outputData);
                if (valid) {
                    Debug.Log("[DMG] Damage Processed: " + damageData.ToString());
                    Game.Event.InvokeNow(onProcessedDamageEventName, damageData.receiverId, outputData);
                }
            }
        }


        public bool Process(IDamageData damageData, out IDamageEventArgs outputData) {
            var valid = Validate(damageData, out var message);
            if (!valid) {
                outputData = null;
                Debug.Log("[DMG] Damage Invalid: " + damageData.ToString() + "\n" + message);
                return false;
            }

            outputData = Calculate(damageData);
            return true;
        }

        public abstract bool Validate(IDamageData damageData, out string message);
        public abstract IDamageEventArgs Calculate(IDamageData damageData);
    }
}