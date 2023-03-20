using System;
using System.Collections.Generic;
using Inventory.Logic;
using UnityEngine;
namespace Inventory
{
    /// <summary>
    /// UI界面的一个控制脚本
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        /// <summary>
        /// ui上显示的格子GameObject
        /// </summary>
        [SerializeField] private SlotUI[] _slotUis;

        private void Start()
        {
            for (int i = 0; i < _slotUis.Length; i++)
            {
                _slotUis[i].slotIndex = i; 
            }
        }

        private void OnEnable()
        {
            EvnetHandler.UpdateInventoryUI += OnUpdateInventoryUI;
        }
        private void OnUpdateInventoryUI(InventoryLocation inventoryType, List<InventoryItem> bagItems)
        {
            switch (inventoryType)
            {
                case InventoryLocation.Player :
                    for (int i = 0; i < _slotUis.Length; i++)
                    {
                        if (bagItems[i].itemAmount > 0 )
                        {
                            // 有东西
                            ItemDetails item = InventoryManager.Instance.GetItemDetails(bagItems[i].itemID);
                            _slotUis[i].UpdateSlot(item,bagItems[i].itemAmount);
                        }
                        else
                        {
                            // 没有东西 就清空
                            _slotUis[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }
        }

        private void OnDisable()
        {
            EvnetHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
        }
    }
}
