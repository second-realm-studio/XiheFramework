using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public static class PlayerSvc {
    public static void SetPlayerPosition(Vector3 position) {
        PlayerHomeController player = Game.Blackboard.GetData<PlayerHomeController>("HomePlayerInstance");
        if (player!=null) {
            player.SetPosition(position);
        }
    }
}