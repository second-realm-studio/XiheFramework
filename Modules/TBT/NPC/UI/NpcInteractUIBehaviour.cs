using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class NpcInteractUIBehaviour : UIBehaviour {
    public NpcInteractUI template;

    public Vector2 offset;

    private Dictionary<Transform, NpcInteractUI> m_NpcUIs = new Dictionary<Transform, NpcInteractUI>();

    public override void Start() {
        base.Start();

        if (template == null) {
            template = Game.Blackboard.GetData<NpcInteractUI>("NpcInteractUI.Template");
        }

        Game.Event.Subscribe("OnNpcInteractUIActivated", OnNpcInteractUIActivated);
    }

    private void Update() {
        UpdatePosition();
    }

    private void OnNpcInteractUIActivated(object sender, object e) {
        if (!(sender is string npcName)) {
            return;
        }

        var trans = Game.Npc.GetNpcTransform(npcName);

        AddNpcInteractUI(npcName);
    }

    void AddNpcInteractUI(string sender) {
        var trans = Game.Npc.GetNpcTransform(sender);
        
        var ui = Instantiate(template, Camera.main.WorldToScreenPoint(trans.position), Quaternion.identity, transform);
        var events = Game.Npc.GetNpcInvokableEvents(sender);
        ui.UpdateFlowEventItems(events);
        
        m_NpcUIs.Add(trans, ui);
    }

    void UpdatePosition() {
        foreach (var trans in m_NpcUIs.Keys) {
            if (Camera.main != null) {
                if (m_NpcUIs[trans].RectTransform != null) {
                    m_NpcUIs[trans].RectTransform.position = Camera.main.WorldToScreenPoint(trans.position) + (Vector3) offset;
                }
            }
        }
    }
}