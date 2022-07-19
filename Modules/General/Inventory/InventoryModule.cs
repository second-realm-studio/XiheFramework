using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework {
    public class InventoryModule : GameModule {
        public List<InventoryItemData> itemDataList;
        private Dictionary<string, InventoryItemData> m_CachedDataList = new Dictionary<string, InventoryItemData>();
        private Dictionary<string, int> m_Inventory = new Dictionary<string, int>(); //counting the amount for every item


        /// <summary>
        /// change item count by delta amount, delta can be negative.
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="delta"></param>
        public void ChangeItemCount(string itemName, int delta) {
            if (m_Inventory.ContainsKey(itemName)) {
                var target = m_Inventory[itemName] + delta;
                m_Inventory[itemName] = Mathf.Clamp(target, 0, m_CachedDataList[itemName].maxCount);
            }
            else {
                Debug.LogError("[INVENTORY] requested item [" + itemName + "] does not exist");
            }
        }

        public int GetCount(string itemName) {
            if (m_Inventory.ContainsKey(itemName)) {
                return m_Inventory[itemName];
            }
            else {
                Debug.LogError("[INVENTORY] requested item [" + itemName + "] does not exist");
            }

            return 0;
        }

        public override void Setup() {
            base.Setup();

            //copy data to dictionary to cache them
            foreach (var itemData in itemDataList) {
                m_CachedDataList.Add(itemData.internalName, itemData);
            }

            //read data from save data
        }

        public override void Update() {
        }

        public override void ShutDown(ShutDownType shutDownType) {
            m_Inventory.Clear();
        }
    }
}