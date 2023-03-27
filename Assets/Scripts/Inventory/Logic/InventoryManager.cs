using System;
using System.Linq;
using Inventory.Data_SO;
using Inventory.Item;
using UnityEngine;
using UnityEngine.Serialization;

namespace Inventory.Logic
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("所有的存储的数据")]
        public ItemDataList_SO _ItemDataListSo;

        [FormerlySerializedAs("_PlayerBag")]
        [Header("背包的数据")]
        public InventoryBag_SO _playerBag;

        private void OnEnable()
        {
            MyEvnetHandler.DropItemEvent += OnDropItemEvent;
        }

        private void OnDisable()
        {
            MyEvnetHandler.DropItemEvent += OnDropItemEvent;

        }
        private void Start()
        {
            MyEvnetHandler.CallUpdateInventoryUI(InventoryLocation.Player,_playerBag.itemInventoryItems);
        }
        // itemID => itemDetails
        public ItemDetails GetItemDetails(int ID)
        {
            return _ItemDataListSo?.itemDetailsList?.Find(i => i.itemID == ID);
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
            MyEvnetHandler.CallUpdateInventoryUI(InventoryLocation.Player,_playerBag.itemInventoryItems);
        }

        /// <summary>
        /// 背包是否有空位
        /// </summary>
        /// <returns></returns>
        private bool CheckBagCapacity()
        {
            foreach (InventoryItem item in _playerBag.itemInventoryItems)
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
            for (int i = 0; i < _playerBag.itemInventoryItems.Count; i++)
            {
                if (_playerBag.itemInventoryItems[i].itemID == id)
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
                InventoryItem inventoryItem = _playerBag.itemInventoryItems[index];
                itemOnWorld.itemAmount += inventoryItem.itemAmount;
            }
            else
            {
                // 结构体默认值为0
                // 容量一直固定在20
                // 需要遍历寻找空位住下
                
                //index = _playerBag.itemInventoryItems.Count + 1;
                for (int i = 0; i < _playerBag.itemInventoryItems.Count; i++)
                {
                    if (_playerBag.itemInventoryItems[i].itemID == 0)
                    {
                        index = i;
                        break;
                    }
                }
            }
            
            _playerBag.itemInventoryItems[index] = itemOnWorld;
        }

        public void SwapItem(int from, int to)
        {
            var currentItem = _playerBag.itemInventoryItems[from];
            var targetItem = _playerBag.itemInventoryItems[to];
            if (targetItem.itemID != 0)
            {
                (_playerBag.itemInventoryItems[from], _playerBag.itemInventoryItems[to]) = (targetItem, currentItem);
            }
            else
            {
                _playerBag.itemInventoryItems[to] = currentItem;
                _playerBag.itemInventoryItems[from] = new InventoryItem();
            }
            
            MyEvnetHandler.CallUpdateInventoryUI(InventoryLocation.Player,_playerBag.itemInventoryItems);
        }

        private void RemoveItem(int ID, int removeAmount)
        {
            var index = CheckItemInBag(ID);
            if (_playerBag.itemInventoryItems[index].itemAmount > removeAmount)
            {
                var amount = _playerBag.itemInventoryItems[index].itemAmount - removeAmount;
                var item = new InventoryItem()
                {
                    itemID = ID,
                    itemAmount = amount
                };
                _playerBag.itemInventoryItems[index] = item;
            }else if (_playerBag.itemInventoryItems[index].itemAmount == removeAmount)
            {
                _playerBag.itemInventoryItems[index] = new InventoryItem();
            }
            
            // 刷新一下界面
            MyEvnetHandler.CallUpdateInventoryUI(InventoryLocation.Player,_playerBag.itemInventoryItems);
        }
        /// <summary>
        /// 道具丢弃出去，就需要在数据库中 -= 这个道具
        /// </summary>
        /// <param name="itemId">道具ID</param>
        /// <param name="pos">世界坐标</param>
        private void OnDropItemEvent(int itemId, Vector3 pos)
        {
            RemoveItem(itemId, 1);
        }

    }
}