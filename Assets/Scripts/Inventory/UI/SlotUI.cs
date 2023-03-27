using Inventory.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Utility;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace Inventory.UI
{
    public class SlotUI : MonoBehaviour,IPointerClickHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        [FormerlySerializedAs("itemImage")]
        [Header("组件获取")]
        [SerializeField]private Image image;
        [FormerlySerializedAs("itemAmountTxt")]
        [FormerlySerializedAs("itemAmount")]
        [SerializeField]private TextMeshProUGUI amountTxt;
        [FormerlySerializedAs("itemSelectImage")]
        [SerializeField]public Image selectImage;
        [FormerlySerializedAs("_button")]
        [SerializeField]private Button button;

        [FormerlySerializedAs("_slotType")]
        [Header("格子类型")]
        [SerializeField] public SlotType slotType;

        /// <summary>
        /// 道具的详细信息
        /// </summary>
        [FormerlySerializedAs("_itemDetails")]
        public ItemDetails itemDetails;
        /// <summary>
        /// 道具的个数
        /// </summary>
        [FormerlySerializedAs("itemAmount")]
        [FormerlySerializedAs("_itemAmount")]
        public int amount;
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
            if (itemDetails == null)
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
                _inventoryUI.UpdateSlotSelected(-1);
                MyEventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
            itemDetails = null;
            image.enabled = false;
            amountTxt.text = "";
            button.interactable = false;
        }

        public void UpdateSlot(ItemDetails item,int amount)
        {
            image.sprite = item.itemIcon;
            this.amount = amount;
            amountTxt.text = this.amount.ToString();
            itemDetails = item;

            button.interactable = true;
            image.enabled = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // 如果格子没有道具就不要被选中啦
            if (itemDetails == null)
            {
                return;
            }
        
            isSelected = !isSelected;
            selectImage.gameObject.SetActive(isSelected);

            _inventoryUI.UpdateSlotSelected(slotIndex);
        
            // 举起的事件逻辑
            // 
            if (slotType == SlotType.Bag)
            {
                // 触发举起的事件
                MyEventHandler.CallItemSelectedEvent(itemDetails,isSelected);
            }
        }
        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="eventData"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (amount != 0)
            {
                _inventoryUI.dragImage.gameObject.SetActive(true);
                _inventoryUI.dragImage.sprite = image.sprite;
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
        public void OnEndDrag(PointerEventData eventData)
        {
            _inventoryUI.dragImage.gameObject.SetActive(false);
            isSelected = false;
            selectImage.gameObject.SetActive(isSelected);
        
            var target = eventData.pointerCurrentRaycast.gameObject;
            if (target != null )
            {
                if (target.GetComponent<SlotUI>() != null)
                {
                    // 碰撞到UI物体了
                    var targetSlot = target.GetComponent<SlotUI>();
                    int targetIndex = targetSlot.slotIndex;

                    // 只是在背包自己身上做数据调换
                    if (SlotType.Bag == slotType && targetSlot.slotType == SlotType.Bag)
                    {
                        InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
                    }
                }
            }
            else
            {
                // Debug.Log(_itemDetails.itemID);
                // if (_itemDetails.canDropped)
                // {
                //     // 这时候是丢弃到地上的
                //     // 地上的坐标
                //     var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                //     EvnetHandler.CallCloneCloneSlotInWorld(_itemDetails.itemID, pos);
                // }
            }
        }
    
    }
}
