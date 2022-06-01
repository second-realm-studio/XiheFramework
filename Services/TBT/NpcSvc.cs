using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public static class NpcSvc {
    public static void ActivateEvent(string npcName, string eventName) {
        Game.Npc.ActivateNpcEvent(npcName, eventName);
    }

    public static void DeactivateEvent(string npcName, string eventName) {
        Game.Npc.DeactivateEvent(npcName, eventName);
    }
}