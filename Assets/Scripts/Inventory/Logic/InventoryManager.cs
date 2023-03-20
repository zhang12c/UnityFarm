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
        
        
    }
}