using UnityEngine;
namespace Inventory.Item
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemShadow : MonoBehaviour
    {
        /// <summary>
        /// 正常的颜色
        /// </summary>
        public SpriteRenderer spriteRender;
        /// <summary>
        /// 阴影的图片
        /// </summary>
        private SpriteRenderer _shadowSpriteRenderer;

        private void Awake()
        {
            _shadowSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _shadowSpriteRenderer.sprite = spriteRender.sprite;
            _shadowSpriteRenderer.color = new Color(0, 0, 0, 0.3f);
        }

    }
}
