using Crop.Logic;
using UnityEngine;
using Utility;

namespace Inventory.Item
{
    public class ItemOnWorld : MonoBehaviour
    {
        public int itemID;
        private SpriteRenderer itemSpriteRenderer;
        public ItemDetails _itemDetails;

        // 图片锚点在底部，而item的碰撞体中心在图片底部，这里需要设置offset将碰撞体包裹住
        private BoxCollider2D _boxCollider2D;
        private void Awake()
        {
            itemSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            if (itemID > 0)
            {
                CreateItem(itemID);
                OffsetBoxCollider();
            }
        }

        private void CreateItem(int ID)
        {
            itemID = ID;
            _itemDetails = Inventory.Logic.InventoryManager.Instance.GetItemDetails(itemID);
            if (_itemDetails != null)
            {
                itemSpriteRenderer.sprite = _itemDetails.itemOnWorldSprite != null ? _itemDetails.itemOnWorldSprite : _itemDetails.itemIcon;
            }

            if (_itemDetails.itemType == ItemType.ReapableScenery)
            {
                gameObject.AddComponent<ReapItem>();
                gameObject.GetComponent<ReapItem>().InitCropData(_itemDetails.itemID);
                
                // 路过杂草可以摇晃
                gameObject.AddComponent<ItemInteractive>();

            }
        }

        public void CloneItem(int ID)
        {
            if (ID > 0)
            {
                CreateItem(ID);
                OffsetBoxCollider();
            }
        }

        /// <summary>
        /// 碰撞体大小随图标大小做自适应
        /// </summary>
        private void OffsetBoxCollider()
        {
            Bounds spriteBounds = itemSpriteRenderer.sprite.bounds;
             
            _boxCollider2D.size = spriteBounds.size;
            _boxCollider2D.offset = new Vector2(0, spriteBounds.center.y);

        }
    }
}
