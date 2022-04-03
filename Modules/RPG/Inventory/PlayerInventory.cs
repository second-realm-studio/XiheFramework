using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XiheFramework;

public class PlayerInventory : GameModule {
    [SerializeField] private List<InventoryItemData> dataCandidates = new List<InventoryItemData>();

    private Dictionary<string, InventoryItemInfo> m_Inventory; //counting the amount for every item


    private void Start() {
        m_Inventory = new Dictionary<string, InventoryItemInfo>();

        /////////////////占位用的/////////////////////////////
        foreach (var itemData in dataCandidates) {
            m_Inventory.Add(itemData.name, new InventoryItemInfo(itemData, 0));
        }

        ///////////////////////////////////////////////////
    }

    public void AddItem(string itemName, int amount) {
        if (!m_Inventory.ContainsKey(itemName)) return;

        m_Inventory[itemName].Count = Mathf.Clamp(m_Inventory[itemName].Count + amount, 0, m_Inventory[itemName].Data.maxCount);
    }

    public void RemoveItem(string itemName, int amount) {
        if (!m_Inventory.ContainsKey(itemName)) return;

        m_Inventory[itemName].Count = Mathf.Clamp(m_Inventory[itemName].Count - amount, 0, m_Inventory[itemName].Data.maxCount);
    }

    public InventoryItemData RetrieveItemData(string itemName) {
        return !m_Inventory.ContainsKey(itemName) ? null : m_Inventory[itemName].Data;
    }

    public bool TryUseItem(string itemName, int amount) {
        if (!m_Inventory.ContainsKey(itemName)) {
            return false;
        }

        var item = m_Inventory[itemName];
        if (item.Count < amount) {
            return false;
        }

        if (item.Data.consumable) {
            item.Count -= amount;
        }

        return true;
    }


    public class InventoryItemInfo {
        public InventoryItemData Data;
        public int Count;

        public InventoryItemInfo(InventoryItemData data, int count) {
            this.Data = data;
            this.Count = count;
        }
    }

    public List<InventoryItemInfo> GetInventoryItemInfos() {
        return m_Inventory.Values.ToList();
    }

    public override void Update() {
    }

    public override void ShutDown(ShutDownType shutDownType) {
        m_Inventory.Clear();
    }
}