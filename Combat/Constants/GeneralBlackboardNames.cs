using XiheFramework.Combat.Base;

namespace XiheFramework.Combat.Constants {
    public static class GeneralBlackboardNames {

        public static string CombatEntity_MaxHp(CombatEntity entity) => $"CombatEntity.{entity.EntityName}[{entity.EntityId}].MaxHP";
        public static string CombatEntity_CurrentHp(CombatEntity entity) => $"CombatEntity.{entity.EntityName}[{entity.EntityId}].CurrentHP";
        public static string CombatEntity_MaxStamina(CombatEntity entity) => $"CombatEntity.{entity.EntityName}[{entity.EntityId}].MaxStamina";
        public static string CombatEntity_CurrentStamina(CombatEntity entity) => $"CombatEntity.{entity.EntityName}[{entity.EntityId}].CurrentStamina";
        public static string CombatEntity_CurrentPosition(CombatEntity entity) => $"CombatEntity.{entity.EntityName}[{entity.EntityId}].CurrentPosition";
        public static string CombatEntity_CurrentScale(CombatEntity entity) => $"CombatEntity.{entity.EntityName}[{entity.EntityId}].CurrentPosition";

        public static string CombatEntity_CurrentLooking(CombatEntity entity) =>
            $"CombatEntity.{entity.EntityName}[{entity.EntityId}].CurrentLooking"; //return true when facing right

        public static string CombatEntity_CurrentActionName(CombatEntity entity) => $"CombatEntity.{entity.EntityName}[{entity.EntityId}].CurrentActionName";

        public static string Buff_CurrentStack(CombatEntity owner, string buffName) => $"Buff.{owner.EntityName}[{owner.EntityId}].{buffName}.CurrentStack";

        //player weapon
    }
}