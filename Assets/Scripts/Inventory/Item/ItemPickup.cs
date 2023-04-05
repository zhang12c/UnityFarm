using UnityEngine;
using Inventory.Item;
using Inventory.Logic;
using Utility;
public class ItemPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        ItemOnWorld item = col.GetComponent<ItemOnWorld>();
        
        // 这个碰撞的就是掉落的道具
        if (item != null)
        {
            if (item._itemDetails.canPickedup)
            {
                InventoryManager.Instance.AddItem(item,true);
                // 音乐
                MyEventHandler.CallPlaySoundEvent(SoundName.Pickup);
            }
        }
    }
}
