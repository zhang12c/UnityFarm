using Inventory.Item;
using UnityEngine;

namespace Inventory.Logic
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        // 所有的存储的数据
        public ItemDataList_SO _ItemDataListSo;
        
        // itemID => itemDetails
        public ItemDetails GetItemDetails(int ID)
        {
            return _ItemDataListSo?.itemDetailsList?.Find(i => i.itemID == ID);
        }

        public void AddItem(ItemOnWorld item, bool DoDestory)
        {
            ItemDetails _itemDetails = GetItemDetails(item.itemID);
            if (_itemDetails != null)
            {
                // 背包里面有道具，递增
                
            }
            else
            {
                // 背包里没有道具，添加
            }
            
            
            if (DoDestory)
            {
                Destroy(item.gameObject);
            }
        }
    }
}