using System.Collections.Generic;
using Inventory.Data_SO;
using Inventory.Item;
using SaveLoad.Data;
using SaveLoad.Logic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Inventory.Logic
{
    public class InventoryManager : Singleton<InventoryManager>,ISaveAble
    {
        [FormerlySerializedAs("_ItemDataListSo")]
        [Header("所有的存储的数据")]
        public ItemDataList_SO itemDataListSo;

        [FormerlySerializedAs("_playerBag")]
        [FormerlySerializedAs("_PlayerBag")]
        [Header("背包的数据")]
        public InventoryBag_SO playerBag;
        
        public InventoryBag_SO playerBagTemp;
        private string _guid;

        private void OnEnable()
        {
            MyEventHandler.DropItemEvent += OnDropItemEvent;
            MyEventHandler.NewAtPlayerPositionEvent += OnNewAtPlayerPositionEvent;
            MyEventHandler.StartNewGameEvent += OnStartNewGameEvent;

        }

        private void OnDisable()
        {
            MyEventHandler.DropItemEvent += OnDropItemEvent;
            MyEventHandler.NewAtPlayerPositionEvent -= OnNewAtPlayerPositionEvent;
            MyEventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        }
        private void Start()
        {
            ISaveAble saveAble = this;
            saveAble.RegisterSaveAble();
        }
        
        private void OnStartNewGameEvent(int obj)
        {
            playerBag = Instantiate(playerBagTemp);
            MyEventHandler.CallUpdateInventoryUI(InventoryLocation.Player,playerBag.itemInventoryItems);
        }
        // itemID => itemDetails
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataListSo?.itemDetailsList?.Find(i => i.itemID == ID);
        }

        public void AddItem(ItemOnWorld item, bool DoDestory)
        {
            // 是否已经有物品
            int hasInBagIndex = CheckItemInBag(item.itemID);
            
            // 背包是否有空位
            if (!CheckBagCapacity())
            {
                return;
            }
            // 有容量背包里也没有
            // 那就添加一个
            AddItemAtIndex(item.itemID, hasInBagIndex, 1);
            
            if (DoDestory)
            {
                Destroy(item.gameObject);
            }
            
            // 更新UI 数据变化了
            MyEventHandler.CallUpdateInventoryUI(InventoryLocation.Player,playerBag.itemInventoryItems);
        }

        public void AddItem(int id)
        {
            // 是否已经有物品
            int hasInBagIndex = CheckItemInBag(id);
            
            // 背包是否有空位
            if (!CheckBagCapacity())
            {
                return;
            }
            // 有容量背包里也没有
            // 那就添加一个
            AddItemAtIndex(id, hasInBagIndex, 1);
            
            // 更新UI 数据变化了
            MyEventHandler.CallUpdateInventoryUI(InventoryLocation.Player,playerBag.itemInventoryItems);
        }

        /// <summary>
        /// 背包是否有空位
        /// </summary>
        /// <returns></returns>
        private bool CheckBagCapacity()
        {
            foreach (InventoryItem item in playerBag.itemInventoryItems)
            {
                if (item.itemID == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否在背包里有道具
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private int CheckItemInBag(int id)
        {
            for (int i = 0; i < playerBag.itemInventoryItems.Count; i++)
            {
                if (playerBag.itemInventoryItems[i].itemID == id)
                {
                    return i;
                }
            }
            return -1;
        }

        private void AddItemAtIndex(int ID, int index, int amount)
        {
            InventoryItem itemOnWorld = new InventoryItem()
            {
                itemID = ID,
                itemAmount = amount,
            };
            
            if (index >= 0)
            {
                InventoryItem inventoryItem = playerBag.itemInventoryItems[index];
                itemOnWorld.itemAmount += inventoryItem.itemAmount;
            }
            else
            {
                // 结构体默认值为0
                // 容量一直固定在20
                // 需要遍历寻找空位住下
                
                //index = _playerBag.itemInventoryItems.Count + 1;
                for (int i = 0; i < playerBag.itemInventoryItems.Count; i++)
                {
                    if (playerBag.itemInventoryItems[i].itemID == 0)
                    {
                        index = i;
                        break;
                    }
                }
            }
            
            playerBag.itemInventoryItems[index] = itemOnWorld;
        }

        public void SwapItem(int from, int to)
        {
            var currentItem = playerBag.itemInventoryItems[from];
            var targetItem = playerBag.itemInventoryItems[to];
            if (targetItem.itemID != 0)
            {
                (playerBag.itemInventoryItems[from], playerBag.itemInventoryItems[to]) = (targetItem, currentItem);
            }
            else
            {
                playerBag.itemInventoryItems[to] = currentItem;
                playerBag.itemInventoryItems[from] = new InventoryItem();
            }
            
            MyEventHandler.CallUpdateInventoryUI(InventoryLocation.Player,playerBag.itemInventoryItems);
        }

        private void RemoveItem(int ID, int removeAmount)
        {
            var index = CheckItemInBag(ID);
            if (playerBag.itemInventoryItems[index].itemAmount > removeAmount)
            {
                var amount = playerBag.itemInventoryItems[index].itemAmount - removeAmount;
                var item = new InventoryItem()
                {
                    itemID = ID,
                    itemAmount = amount
                };
                playerBag.itemInventoryItems[index] = item;
            }else if (playerBag.itemInventoryItems[index].itemAmount == removeAmount)
            {
                playerBag.itemInventoryItems[index] = new InventoryItem();
            }
            
            // 刷新一下界面
            MyEventHandler.CallUpdateInventoryUI(InventoryLocation.Player,playerBag.itemInventoryItems);
        }
        /// <summary>
        /// 道具丢弃出去，就需要在数据库中 -= 这个道具
        /// </summary>
        /// <param name="itemId">道具ID</param>
        /// <param name="pos">世界坐标</param>
        private void OnDropItemEvent(int itemId, Vector3 pos,ItemType type)
        {
            RemoveItem(itemId, 1);
        }
        
        /// <summary>
        /// 生成收获道具
        /// </summary>
        /// <param name="itemID">收获id</param>
        private void OnNewAtPlayerPositionEvent(int itemID)
        {
            AddItem(itemID);
        }
        
        // 保存数据相关
        public GameSaveData GenerateSaveData()
        {
            var key = playerBag.name;
            {
                GameSaveData saveData = new GameSaveData
                {
                    inventoryDict = new Dictionary<string, List<InventoryItem>>
                    {
                        [key] = playerBag.itemInventoryItems
                    }
                };
                return saveData;
            }
        }
        public void RestoreData(GameSaveData saveData)
        {
            playerBag = Instantiate(playerBagTemp);
            if (saveData.inventoryDict.TryGetValue(playerBag.name,out List<InventoryItem> value))
            {
                playerBag.itemInventoryItems = value;
            }
            
            MyEventHandler.CallUpdateInventoryUI(InventoryLocation.Player,playerBag.itemInventoryItems);
        }
        public string GUID
        {
            get
            {
                return GetComponent<DataGUID>().guid;
            }
        }
    }
}