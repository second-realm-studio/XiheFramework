using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class EntityModule : GameModule {
    private Dictionary<string, Entity> m_Entities = new Dictionary<string, Entity>();

    public void RegisterEntity(string id, Entity entity) {
        if (m_Entities.ContainsKey(id)) {
            m_Entities[id] = entity;
        }
        else {
            m_Entities.Add(id, entity);
        }
    }

    public Entity GetEntity(string entityName) {
        var id = new Hash128();
        id.Append(entityName);
        if (!m_Entities.ContainsKey(id.ToString())) {
            Game.Log.LogErrorFormat("[ENTITY] Entity : {0} is not Existed", entityName);
            return null;
        }

        return m_Entities[id.ToString()];
    }

    public override void Update() {
    }

    public override void ShutDown(ShutDownType shutDownType) {
    }
}