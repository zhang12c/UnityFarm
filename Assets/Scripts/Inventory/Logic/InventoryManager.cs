using Inventory.Data_SO;
using Inventory.Item;
using UnityEngine;

namespace Inventory.Logic
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("所有的存储的数据")]
        public ItemDataList_SO _ItemDataListSo;

        [Header("背包的数据")]
        public InventoryBag_SO _PlayerBag;
        
        // itemID => itemDetails
        public ItemDetails GetItemDetails(int ID)
        {
            return _ItemDataListSo?.itemDetailsList?.Find(i => i.itemID == ID);
        }

        public void AddItem(ItemOnWorld item, bool DoDestory)
        {
            // 背包是否有空位
            InventoryItem inventoryItem = new InventoryItem()
            {
                itemID = item.itemID,
                itemAmount = 1,
            };

            _PlayerBag.itemInventoryItems[0] = inventoryItem;
            // 是否已经有物品
            
            
            if (DoDestory)
            {
                Destroy(item.gameObject);
            }
        }

        public bool CheckBagCapacity()
        {
            
            return false;
        }
        
    }
}