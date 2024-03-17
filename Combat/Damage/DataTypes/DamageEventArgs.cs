using UnityEngine;

namespace XiheFramework.Combat.Damage.DataTypes {
    public struct DamageEventArgs {
        public uint senderId;
        public string senderName;

        public uint receiverId;
        public string receiverName;

        public float rawHealthDamage;
        public float rawStaminaDamage;
        public RawDamageType damageType;
        public float healthDamage;
        public float staminaDamage;

        public Vector3 forceDirection;
        public float forceMagnitude;
        public float stunDuration;
    }
}