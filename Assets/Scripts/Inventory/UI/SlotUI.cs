using Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class SlotUI : MonoBehaviour,IPointerClickHandler
{
    [Header("组件获取")]
    [SerializeField]private Image itemImage;
    [SerializeField]private TextMeshProUGUI itemAmount;
    [SerializeField]public Image itemSelectImage;
    [SerializeField]private Button _button;

    [Header("格子类型")]
    [SerializeField] private SlotType _slotType;

    /// <summary>
    /// 道具的详细信息
    /// </summary>
    public ItemDetails _itemDetails;
    /// <summary>
    /// 道具的个数
    /// </summary>
    public int _itemAmount;
    /// <summary>
    /// 在背包中的位置
    /// </summary>
    public int slotIndex;

    // 是否被选中了
    public bool isSelected;
    
    private InventoryUI _inventoryUI => GetComponentInParent<InventoryUI>();


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

    public void OnPointerClick(PointerEventData eventData)
    {
        // 如果格子没有道具就不要被选中啦
        if (_itemAmount == 0)
        {
            return;
        }
        
        isSelected = !isSelected;
        itemSelectImage.gameObject.SetActive(isSelected);

        _inventoryUI.UpdateSlotSelected(slotIndex);
    }
}
