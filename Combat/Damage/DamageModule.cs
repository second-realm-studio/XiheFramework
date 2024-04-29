using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Combat.Damage.DataTypes;
using XiheFramework.Core;
using XiheFramework.Core.Base;

namespace XiheFramework.Combat.Damage {
    public class DamageModule : GameModule {
        public readonly string OnProcessedDamageEventName = "Damage.OnProcessedDamage";

        public DamageProcessorBase damageProcessor;

        private Queue<DamageData> m_DamageQueue = new();

        public void SetDamageProcessor(DamageProcessorBase processor) {
            damageProcessor = processor;
        }

        public void RegisterDamage(DamageData damageData) {
            m_DamageQueue.Enqueue(damageData);

            if (enableDebug) {
                Debug.Log(
                    $"[DMG][RGST]({damageData.senderId})->({damageData.receiverId}):[{damageData.rawHealthDamage}]HP.[{damageData.rawStaminaDamage}]SP.Force:{damageData.rawDamageForce}");
            }
        }

        public void RegisterDamage(uint senderId, uint receiverId, float rawDamage, float rawStaminaDamage, RawDamageType rawDamageType, Vector3 force = default,
            float stunDuration = 0f, params string[] damageTags) {
            if (!GameCore.Entity.IsEntityExisted(senderId)) {
                Debug.LogError($"[DMG] Damage register failed: senderId {senderId} is not existed");
                return;
            }

            if (!GameCore.Entity.IsEntityExisted(receiverId)) {
                Debug.LogError($"[DMG] Damage register failed: receiverId {receiverId} is not existed");
                return;
            }

            if (enableDebug) {
                Debug.Log($"[DMG][RGST]{senderId}->{receiverId}:[{rawDamage}][{rawDamageType}]HP,[{rawStaminaDamage}]SP.Force:{force}");
            }

            DamageData damageData = new DamageData(senderId, receiverId, rawDamage, rawStaminaDamage, rawDamageType, force, stunDuration, damageTags);
            m_DamageQueue.Enqueue(damageData);
        }


        public override void OnLateUpdate() {
            //TODO: (Maybe) change to concurrent queue
            if (m_DamageQueue.Count > 0) {
                var damageData = m_DamageQueue.Dequeue();
                var valid = damageProcessor.Process(damageData, out var outputData);
                if (valid) {
                    GameCore.Event.InvokeNow(OnProcessedDamageEventName, damageData.receiverId, outputData);

                    if (enableDebug) {
                        Debug.Log(
                            $"[DMG][FNAL]{outputData.senderName}({outputData.senderId})->{outputData.receiverName}({outputData.receiverId}):({outputData.damageType})[{outputData.healthDamage}]HP.[{outputData.staminaDamage}]SP.Force:{outputData.forceDirection * outputData.forceMagnitude}. Stun:{outputData.stunDuration}");
                    }
                }
                else {
                    if (enableDebug) {
                        Debug.Log(
                            $"[DMG][FNAL]Damage process failed, RawData:{damageData.senderId}->{damageData.receiverId}:[{damageData.rawHealthDamage}][{damageData.rawDamageType}]HP,[{damageData.rawStaminaDamage}]SP.Force:{damageData.rawDamageForce}");
                    }
                }
            }
        }
    }
}