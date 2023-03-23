using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotToolTip : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    // 显示的内容
    private SlotUI _slotUI;
    private InventoryUI _inventoryUI;
    private void Awake()
    {
        _slotUI = GetComponent<SlotUI>();
        _inventoryUI = GetComponentInParent<InventoryUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var itemDetails = _slotUI._itemDetails;
        if (_slotUI._itemAmount > 0)
        {
            _inventoryUI.itemToolTip.gameObject.SetActive(true);
            _inventoryUI.itemToolTip.SetupToolTip(itemDetails,_slotUI._slotType);
            var slotRect = _slotUI.GetComponent<RectTransform>();
            _inventoryUI.itemToolTip.transform.position = new Vector3(transform.position.x,transform.position.y + slotRect.rect.height / 2,transform.position.z);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        var isShow = _inventoryUI.itemToolTip.gameObject.activeSelf;
        if (isShow)
        {
            _inventoryUI.itemToolTip.gameObject.SetActive(!isShow);
        }
    }
}
