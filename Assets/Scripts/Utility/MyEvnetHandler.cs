using System;
using System.Collections.Generic;
using UnityEngine;

public static class MyEvnetHandler
{
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    /// <summary>
    /// 重新绘制一下背包数据
    /// </summary>
    /// <param name="location">背包的类别</param>
    /// <param name="inventoryItems">背包的数据</param>
    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> inventoryItems)
    {
        UpdateInventoryUI?.Invoke(location,inventoryItems);
    }

    /// <summary>
    /// 将物体克隆到场景中
    /// </summary>
    public static event Action<int, Vector3> CloneSlotInWorld;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemId">道具Id</param>
    /// <param name="pos">坐标位置</param>
    public static void CallCloneCloneSlotInWorld(int itemId, Vector3 pos)
    {
        CloneSlotInWorld?.Invoke(itemId,pos);
    }

    /// <summary>
    /// 点击slotUI的时候
    /// 触发的举起事件
    /// </summary>
    public static event Action<ItemDetails, bool> ItemSelectedEvent;
    public static void CallItemSelectedEvent(ItemDetails itemDetails, bool isHold)
    {
        ItemSelectedEvent?.Invoke(itemDetails,isHold);
    }
    
}
