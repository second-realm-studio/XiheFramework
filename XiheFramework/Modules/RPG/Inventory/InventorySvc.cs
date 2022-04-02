using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework.RPG {
    public static class InventorySvc {
        public static void InitItem(Action onUse) {
            var item = new InventoryItem() {
                OnUse = onUse,
            };
        }

        public static void AddItem(string itemName, int amount = 1) {
            //Rpg.Inventory.AddItem(itemName, amount);
        }

        public static bool TryUseItem(string itemName, int amount) {
           // return Rpg.Inventory.TryUseItem(itemName, amount);
           return false;
        }

        public static void UpdateUI() {
            //Rpg.Inventory.UpdateInventoryUI();
        }
    }
}