using Inventory;
using Inventory.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class SlotUI : MonoBehaviour,IPointerClickHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    [Header("组件获取")]
    [SerializeField]private Image itemImage;
    [FormerlySerializedAs("itemAmount")]
    [SerializeField]private TextMeshProUGUI itemAmountTxt;
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


    // private void Start()
    // {
    //     isSelected = false;
    //     if (_itemDetails.itemID == 0)
    //     {
    //         UpdateEmptySlot();
    //     }
    // }

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
        itemAmountTxt.text = "";
        _button.interactable = false;
    }

    public void UpdateSlot(ItemDetails item,int amount)
    {
        itemImage.sprite = item.itemIcon;
        _itemAmount = amount;
        itemAmountTxt.text = _itemAmount.ToString();
        _itemDetails = item;

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
    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_itemAmount != 0)
        {
            _inventoryUI.dragImage.gameObject.SetActive(true);
            _inventoryUI.dragImage.sprite = itemImage.sprite;
            _inventoryUI.dragImage.SetNativeSize();
            
            isSelected = true;
            _inventoryUI.UpdateSlotSelected(slotIndex);
        }
    }
    
    /// <summary>
    /// 拖拽中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        _inventoryUI.dragImage.transform.position = Input.mousePosition;
    }
    
    /// <summary>
    /// 结束拖拽
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnEndDrag(PointerEventData eventData)
    {
        _inventoryUI.dragImage.gameObject.SetActive(false);
        isSelected = false;
        itemSelectImage.gameObject.SetActive(isSelected);
        
        var target = eventData.pointerCurrentRaycast.gameObject;
        if (target != null )
        {
            if (target.GetComponent<SlotUI>() != null)
            {
                // 碰撞到UI物体了
                var targetSlot = target.GetComponent<SlotUI>();
                int targetIndex = targetSlot.slotIndex;

                // 只是在背包自己身上做数据调换
                if (SlotType.Bag == _slotType && targetSlot._slotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
                }
            }
        }
        else
        {
            Debug.Log(_itemDetails.itemID);
            if (_itemDetails.canDropped)
            {
                // 这时候是丢弃到地上的
                // 地上的坐标
                var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                EvnetHandler.CallCloneCloneSlotInWorld(_itemDetails.itemID, pos);
            }
        }
    }
}
