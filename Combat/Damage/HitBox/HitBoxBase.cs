using System;
using UnityEngine;
using UnityEngine.Serialization;
using XiheFramework.Combat.Damage.Interfaces;
using XiheFramework.Core.Entity;
using XiheFramework.Core.LogicTime;
using XiheFramework.Runtime;
using XiheFramework.Utility.Extension;

namespace XiheFramework.Combat.Damage.HitBox {
    public abstract class HitBoxBase : MonoBehaviour {
        public uint OwnerId { get; private set; }

        public LayerMask hitLayerMask;

        public void EnableHitBox(uint owner) {
            OwnerId = owner;
            OnEnableHitBox();
        }

        public void DisableHitBox() => OnDisableHitBox();

        protected abstract void OnEnableHitBox();

        protected abstract void OnDisableHitBox();
    }
}