using UnityEngine;
using XiheFramework.Combat.Base;
using XiheFramework.Core;

namespace XiheFramework.Combat.CameraBehaviour {
    public class CameraSwitchZone : MonoBehaviour {
        public CameraEntity cameraEntity;
        public LayerMask playerLayerMask;
        public uint[] playerEntityIds;

        private void OnTriggerEnter(Collider other) {
            if (cameraEntity == null) {
                return;
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("HurtBox")) {
                foreach (var id in playerEntityIds) {
                    var player = GameCore.Entity.GetEntity<CombatEntity>(id);
                    if (other.gameObject == player.gameObject) {
                        GameCombat.Camera.SetFocusedCamera(cameraEntity.EntityId);
                    }
                }
            }
        }
    }
}