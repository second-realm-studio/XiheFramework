using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Modules.Base;

namespace XiheFramework.Modules.Entity {
    public class EntityModule : GameModule {
        private readonly Dictionary<string, Entity> m_Entities = new();

        public override void Update() { }

        public void RegisterEntity(string id, Entity entity) {
            if (m_Entities.ContainsKey(id))
                m_Entities[id] = entity;
            else
                m_Entities.Add(id, entity);
        }

        public Entity GetEntity(string entityName) {
            var id = new Hash128();
            id.Append(entityName);
            if (!m_Entities.ContainsKey(id.ToString())) {
                Debug.LogErrorFormat("[ENTITY] Entity : {0} is not Existed", entityName);
                return null;
            }

            return m_Entities[id.ToString()];
        }

        public override void ShutDown(ShutDownType shutDownType) { }
    }
}