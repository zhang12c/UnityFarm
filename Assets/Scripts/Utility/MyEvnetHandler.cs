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

    /// <summary>
    /// 每一分钟执行一次
    /// </summary>
    public static event Action<int, int> GameMinuteEvent;
    public static void CallGameMinuteEvent(int m, int h)
    {
        GameMinuteEvent?.Invoke(m,h);
    }
    /// <summary>
    /// 每小时执行一次
    /// </summary>
    public static event Action<int, int,int,int,Season> GameDateEvent;
    public static void CallGameDateEvent(int h, int d, int m,int y,Season season)
    {
        GameDateEvent?.Invoke(h,d,m,y,season);
    }
    /// <summary>
    /// 场景切换
    /// </summary>
    public static event Action<string,Vector3> SceneTransitionEvent;
    public static void CallSceneTransitionEvent(string sceneName,Vector3 toPos)
    {
        SceneTransitionEvent?.Invoke(sceneName,toPos);
    }
    
    

}
