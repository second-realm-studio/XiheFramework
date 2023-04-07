using UnityEngine;
using XiheFramework.Modules.Base;
using XiheFramework.Modules.Entity;

namespace XiheFramework.Services {
    public static class EntitySvc {
        public static Entity GetEntity(string entityName) {
            return Game.Entity.GetEntity(entityName);
        }

        public static void SetupEntity(GameObject owner, string entityName) {
            var containEntity = owner.TryGetComponent<Entity>(out var entity);
            if (containEntity) entity.InitializeEntity(entityName);
        }
    }
}