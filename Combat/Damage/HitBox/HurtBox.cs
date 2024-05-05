using UnityEngine;
using XiheFramework.Combat.Base;

namespace XiheFramework.Combat.Damage.HitBox {
    public class HurtBox : MonoBehaviour {
        public CombatEntity owner;

        private void Start() {
            if (owner == null) {
                owner = GetComponentInParent<CombatEntity>();
            }
        }

#if UNITY_EDITOR
        private void OnValidate() {
            if (owner == null) {
                owner = GetComponentInParent<CombatEntity>();
            }
        }
#endif
    }
}