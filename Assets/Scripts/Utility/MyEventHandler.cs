using System;
using System.Collections.Generic;
using UnityEngine;
namespace Utility
{
    public static class MyEventHandler
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
        /// 将物体克隆到场景中
        /// </summary>
        public static event Action<int, Vector3> DropItemEvent;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId">道具Id</param>
        /// <param name="pos">坐标位置</param>
        public static void CallDropItemEvent(int itemId, Vector3 pos)
        {
            DropItemEvent?.Invoke(itemId,pos);
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
        /// 每天执行一次
        /// </summary>
        public static event Action<int,Season> GameDayEvent;
        public static void CallGameDayEvent(int d,Season season)
        {
            GameDayEvent?.Invoke(d,season);
        }
        /// <summary>
        /// 场景切换
        /// </summary>
        public static event Action<string,Vector3> SceneTransitionEvent;
        public static void CallSceneTransitionEvent(string sceneName,Vector3 toPos)
        {
            SceneTransitionEvent?.Invoke(sceneName,toPos);
        }

        /// <summary>
        /// 场景卸载之前需要做的事件
        /// </summary>
        public static event Action BeforeSceneUnloadEvent;
        public static void CallBeforeSceneUnloadEvent()
        {
            BeforeSceneUnloadEvent?.Invoke();
        }
    
        /// <summary>
        /// 场景卸载之后需要做的事件
        /// </summary>
        public static event Action AfterSceneLoadEvent;
        public static void CallAfterSceneUnloadEvent()
        {
            AfterSceneLoadEvent?.Invoke();
        }
    
        /// <summary>
        /// 瞬移
        /// </summary>
        public static event Action<Vector3> MoveToPos;
        public static void CallMoveToPos(Vector3 pos)
        {
            MoveToPos?.Invoke(pos);
        }

        /// <summary>
        /// 鼠标左键或者右键点击的事件
        /// </summary>
        public static event Action<Vector3, ItemDetails> MouseClickedEvent;
        public static void CallMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
        {
            MouseClickedEvent?.Invoke(pos,itemDetails);
        }
        /// <summary>
        /// 播放动画之后
        /// </summary>
        public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimation;
        public static void CallExecuteActionAfterAnimation(Vector3 pos, ItemDetails itemDetails)
        {
            ExecuteActionAfterAnimation?.Invoke(pos,itemDetails);
        }

    }
}
