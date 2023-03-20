using System.Collections.Generic;
using UnityEngine;
namespace Inventory.Data_SO
{
    [CreateAssetMenu(fileName = "InventoryBag_SO",menuName = "Inventory/InventoryBag_SO")]
    public class InventoryBag_SO : ScriptableObject
    {
        public List<InventoryItem> itemInventoryItems;
        
    }
}