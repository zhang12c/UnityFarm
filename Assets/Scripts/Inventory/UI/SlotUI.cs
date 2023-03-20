using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [Header("组件获取")]
    [SerializeField]private Image itemImage;
    [SerializeField]private TextMeshProUGUI itemAmount;
    [SerializeField]private Image itemSelectImage;
    [SerializeField]private Button _button;

    [Header("格子类型")]
    [SerializeField] private SlotType _slotType;

    public ItemDetails _itemDetails;
    public int _itemAmount;
    public int slotIndex;

    // 是否被选中了
    public bool isSelected;

    private void Start()
    {
        isSelected = false;
        if (_itemDetails.itemID == 0)
        {
            UpdateEmptySlot();
        }
    }

    /// <summary>
    /// 初始化slot 为空格子
    /// </summary>
    public void UpdateEmptySlot()
    {
        if (isSelected)
        {
            isSelected = false;
        }
        itemImage.enabled = false;
        itemAmount.text = "";
        _button.interactable = false;
    }

    public void UpdateSlot(ItemDetails item,int amount)
    {
        itemImage.sprite = item.itemIcon;
        _itemAmount = amount;
        itemAmount.text = _itemAmount.ToString();

        _button.interactable = true;
        itemImage.enabled = true;

    }

}
