using System;
using System.Collections.Generic;
using Inventory.Logic;
using UnityEngine;
using UnityEngine.UI;
namespace Inventory
{
    /// <summary>
    /// UI界面的一个控制脚本
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("背包UI按钮逻辑")]
        [SerializeField] private GameObject _bagUI;
        [SerializeField] private Button _bagBtn;
        private bool bagIsOpen;
        
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
            // 判断背包UI是否是被激活的状态
            bagIsOpen = _bagUI.activeInHierarchy;
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

        /// <summary>
        /// 通过按b来打开背包UI
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                OpenBagUI();
            }
        }

        /// <summary>
        /// 打开UI背包
        /// </summary>
        public void OpenBagUI()
        {
            bagIsOpen = !bagIsOpen;
            _bagUI.SetActive(bagIsOpen);
        }
        /// <summary>
        /// 选中唯一 单选效果
        /// </summary>
        /// <param name="slotIndex"></param>
        public void UpdateSlotSelected(int slotIndex)
        {
            for (int i = 0; i < _slotUis.Length; i++)
            {
                if (i == slotIndex && _slotUis[i].isSelected)
                {
                    _slotUis[i].itemSelectImage.gameObject.SetActive(true);
                }
                else
                {
                    _slotUis[i].itemSelectImage.gameObject.SetActive(false);
                    _slotUis[i].isSelected = false;
                }
            }
        }
    }
}
