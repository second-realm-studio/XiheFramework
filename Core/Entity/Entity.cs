using UnityEngine;
using XiheFramework.Modules.Base;

namespace XiheFramework.Modules.Entity {
    public class Entity : MonoBehaviour {
        private string m_EntityId;
        private string m_EntityName;

        public void InitializeEntity(string entityName) {
            m_EntityName = entityName;
            var hash = new Hash128();
            hash.Append(m_EntityName);
            m_EntityId = hash.ToString();
            Game.Entity.RegisterEntity(m_EntityId, this);
        }
    }
}