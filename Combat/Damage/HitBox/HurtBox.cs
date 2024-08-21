using System;
using UnityEngine;
using UnityEngine.Serialization;
using XiheFramework.Core.Entity;

namespace XiheFramework.Combat.Damage.HitBox {
    public class HurtBox : MonoBehaviour {
        public GameEntity owner;

#if UNITY_EDITOR
        private void OnValidate() {
            if (owner == null) {
                owner = GetComponent<GameEntity>();
            }

            if (owner == null) {
                owner = GetComponentInParent<GameEntity>();
            }
        }
#endif
    }
}