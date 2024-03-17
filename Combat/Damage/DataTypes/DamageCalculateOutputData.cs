using UnityEngine;

namespace XiheFramework.Combat.Damage.DataTypes {
    public struct DamageCalculateOutputData {
        public float calculatedHealthDamage;
        public float calculatedStaminaDamage;
        public Vector3 forceDirectionWorldSpace;
    }
}