using System;
using UnityEngine;
using Utility;
namespace Inventory.UI
{
    [RequireComponent(typeof(SlotUI))]
    public class ActionBarButton : MonoBehaviour
    {
        public KeyCode key;

        private SlotUI _slotUI;

        private void Awake()
        {
            _slotUI = GetComponent<SlotUI>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(key))
            {
                if (_slotUI.itemDetails != null)
                {
                    _slotUI.isSelected = !_slotUI.isSelected;
                    if (_slotUI.isSelected)
                    { 
                        _slotUI.InventoryUI.UpdateSlotSelected(_slotUI.slotIndex);
                    }
                    else
                    {
                        _slotUI.InventoryUI.UpdateSlotSelected(-1);
                    }
                    
                    MyEventHandler.CallItemSelectedEvent(_slotUI.itemDetails,_slotUI.isSelected);
                }
            }
        }
    }
}