using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    /// <summary>
    /// 3中不同状态的鼠标图片
    /// </summary>
    public Sprite normal, tool, seed, item;

    private Sprite currentSprite;
    private Image cursorImage;
    private RectTransform cursorCanvas;

    private void OnEnable()
    {
        MyEvnetHandler.ItemSelectedEvent += OnItemSelectedEvent;
    }
    private void Start()
    {
        cursorCanvas = GameObject.FindWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.Find("CursorImage")?.GetComponent<Image>();
    }

    private void Update()
    {
        if (cursorCanvas != null)
        {
            cursorImage.transform.position = Input.mousePosition;
            if (InteractWithUI())
            {
                SetCursorImage(currentSprite);
            }
            else
            {
                SetCursorImage(normal);
            }
        }
    }

    private void OnDisable()
    {
        MyEvnetHandler.ItemSelectedEvent -= OnItemSelectedEvent;
    }
    
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (!isSelected)
        {
            currentSprite = normal;
            return;
        }
        currentSprite = itemDetails.itemType switch
        {
            // TODO 补全图标
            ItemType.seed => seed,
            ItemType.Commodity => item,
            ItemType.ChopTool => tool,
            _ => normal,
        };

    }

    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1);
    }

    /// <summary>
    /// 是否与UI有互动的 true 是的
    /// </summary>
    /// <returns></returns>
    private bool InteractWithUI()
    {
        // pointer 是否与EventSystem 是否有互动
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }
}
