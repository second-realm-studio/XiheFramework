using UnityEngine;

namespace XiheFramework.Combat.Damage.DataTypes {
    public struct DamageData {
        //sender info
        public uint senderId;

        //receiver info
        public uint receiverId;

        //damage info
        public float rawHealthDamage;
        public float rawStaminaDamage;
        public RawDamageType rawDamageType;

        /// <summary>
        /// damage force in "sender to receiver direction" as +z , Vector3.up as +y space
        /// </summary>
        public Vector3 rawDamageForce;

        public float stunDuration;

        public string[] damageTags; //TODO: if gc is a problem, change to array or long string

        public DamageData(uint senderId, uint receiverId, float rawHealthDamage, float rawStaminaDamage, RawDamageType rawDamageType, Vector3 rawDamageForce, float stunDuration,
            string[] damageTags) {
            this.senderId = senderId;
            this.receiverId = receiverId;
            this.rawHealthDamage = rawHealthDamage;
            this.rawStaminaDamage = rawStaminaDamage;
            this.rawDamageType = rawDamageType;
            this.rawDamageForce = rawDamageForce;
            this.stunDuration = stunDuration;
            this.damageTags = damageTags;
        }
    }
}