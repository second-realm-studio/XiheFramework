using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class Entity : MonoBehaviour {
    private string m_EntityName;
    private string m_EntityId;

    public void InitializeEntity(string entityName) {
        m_EntityName = entityName;
        var hash = new Hash128();
        hash.Append(m_EntityName);
        m_EntityId = hash.ToString();
        Game.Entity.RegisterEntity(m_EntityId, this);
    }
}