using System.Collections;
using System.Collections.Generic;
using FlowCanvas;
using UnityEngine;

[CreateAssetMenu(menuName = "Xihe/Inventory/Inventory Item Data",fileName = "New Item Data")]
public class InventoryItemData :ScriptableObject {
    public string itemName;
    public string description;
    public int maxCount;
    public bool consumable;
    public Sprite icon;

    //public GameObject prefab;
}
