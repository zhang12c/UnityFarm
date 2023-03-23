using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Inventory
{
    public class ItemToolTip : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _itemName;
        [SerializeField] private TextMeshProUGUI _itemType;
        [SerializeField] private TextMeshProUGUI _itemDescrtion;
        [SerializeField] private Text _itemValue;

        public void SetupToolTip(ItemDetails itemDetails,SlotType slotType)
        {
            _itemName.text = itemDetails.itemName;
            _itemType.text = GetItemType(itemDetails.itemType);
            _itemDescrtion.text = itemDetails.itemDescription;

            if (itemDetails.itemType == ItemType.seed || itemDetails.itemType == ItemType.Commodity || itemDetails.itemType == ItemType.Furniture)
            {
                _itemValue.transform.parent.gameObject.SetActive(true);

                int price = itemDetails.itemPrice;
                if (slotType != SlotType.Bag)
                {
                    price = (int)(price * itemDetails.sellPercentage);
                }
                _itemValue.text = price.ToString();
            }
            else
            {
                _itemValue.transform.parent.gameObject.SetActive(false);
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        private string GetItemType(ItemType itemType)
        {
            return itemType switch
            {
                ItemType.seed => "种子",
                ItemType.Commodity => "商品",
                ItemType.Furniture => "家具",
                ItemType.HoeTool => "锄头",
                ItemType.ChopTool => "斧头",
                ItemType.BreakTool => "铁镐",
                ItemType.ReapTool => "镰刀",
                ItemType.WaterTool => "浇水桶",
                ItemType.CollectTool => "收集篮",
                ItemType.ReapableScenery => "收集柜",
                _ => "无",
            };
        }
    }
}