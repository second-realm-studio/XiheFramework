using System;
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

    public static void MoveTo(string npcName, Vector3 destination, bool includeDiagonal) {
        if (Game.Npc.NpcExist(npcName)) {
            Game.Npc.GetNpc(npcName).RequestPath(destination, includeDiagonal);
        }
    }

    public static void MoveTo(string npcName, int x, int y, bool includeDiagonal) {
        if (Game.Npc.NpcExist(npcName)) {
            Game.Npc.GetNpc(npcName).RequestPath(x, y, includeDiagonal);
        }
    }

    public static void FaceTo(string npcName, Vector3 target) {
        if (Game.Npc.NpcExist(npcName)) {
            Game.Npc.GetNpc(npcName).FaceAt(target);
        }
    }
}