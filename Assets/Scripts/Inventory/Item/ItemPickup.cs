using UnityEngine;
using Inventory.Item;
using Inventory.Logic;
public class ItemPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        ItemOnWorld item = col.GetComponent<ItemOnWorld>();
        
        // 这个碰撞的就是掉落的道具
        if (item != null)
        {
            if (item._itemDetails.canPickedup == true)
            {
                InventoryManager.Instance.AddItem(item,true);
            }
        }
    }
}
