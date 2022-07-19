using System.Collections;
using System.Collections.Generic;
using FlowCanvas;
using UnityEngine;

namespace XiheFramework {
    [CreateAssetMenu(menuName = "Xihe/Inventory/Inventory Item Data", fileName = "New Item Data")]
    public class InventoryItemData : ScriptableObject {
        public string internalName;
        public string displayName;

        [TextArea]
        public string description;

        public int maxCount;
        public bool consumable;
        public Sprite icon;

        [TextArea]
        public string note;

        //public GameObject prefab;
    }
}