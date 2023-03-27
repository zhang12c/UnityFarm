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
    
    // 鼠标检测 
    // 鼠标滑动到某一块地上
    // 自动识别出这块地能拿来做啥
    private Camera mainCamera;
    private Grid currentGrid;
    private Vector3 mouseWorldPos;
    private Vector3Int mouseGirdPos;
    /// <summary>
    /// 鼠标的可用性
    /// </summary>
    private bool cursorEnable = false;
    /// <summary>
    /// 鼠标在这个位置上是否是可用的
    /// </summary>
    private bool cursorPositionValid = false;
    /// <summary>
    /// 当前选中的道具
    /// </summary>
    private ItemDetails currentItem;

    private Transform playerTransform
    {
        get
        {
            return FindObjectOfType<Player>().transform;
        }
    }
    private void OnEnable()
    {
        MyEvnetHandler.ItemSelectedEvent += OnItemSelectedEvent;
        MyEvnetHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        MyEvnetHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
    }
    private void Start()
    {
        cursorCanvas = GameObject.FindWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.Find("CursorImage")?.GetComponent<Image>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (cursorCanvas != null)
        {
            cursorImage.transform.position = Input.mousePosition;
            if (!InteractWithUI()&& cursorEnable)
            {
                SetCursorImage(currentSprite);
                CheckCursorValid();
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
        MyEvnetHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        MyEvnetHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;

    }
    private void OnAfterSceneLoadEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
    }
    
    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (!isSelected)
        {
            currentSprite = normal;
            currentItem = null;
            cursorEnable = false;
            return;
        }
        currentSprite = itemDetails.itemType switch
        {
            ItemType.seed => seed,
            ItemType.Commodity => item,
            ItemType.ChopTool => tool,
            ItemType.HoeTool => tool,
            ItemType.WaterTool => tool,
            ItemType.BreakTool => tool,
            ItemType.CollectTool => tool,
            ItemType.ReapTool => tool,
            ItemType.Furniture => tool,
            _ => normal,
        };

        currentItem = itemDetails;
        cursorEnable = true;

    }

    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1);
    }

    #region 鼠标的样式设置
    /// <summary>
    /// 鼠标的可用
    /// </summary>
    private void SetCursorValid()
    {
        cursorImage.color = new Color(1, 1, 1, 1);
        cursorPositionValid = true;
    }
    
    /// <summary>
    /// 鼠标不可用
    /// </summary>
    private void SetCursorInvalid()
    {
        cursorImage.color = new Color(1, 0, 0, 0.5f);
        cursorPositionValid = false;
    }
  #endregion

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

    /// <summary>
    /// 需要实时检测鼠标的坐标
    /// 在Update 中执行
    /// </summary>
    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,0));
        mouseGirdPos = currentGrid.WorldToCell(mouseWorldPos);
        
        // 道具范围逻辑
        var playerPos = currentGrid.WorldToCell(playerTransform.position);
        // 如果鼠标超出了道具的使用范围，就直接不可用了
        if (Mathf.Abs(mouseGirdPos.x - playerPos.x) > currentItem.itemUseRadius || Mathf.Abs(mouseGirdPos.y - playerPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInvalid();
            return;
        }

        var currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGirdPos);
        if (currentTile != null)
        {
            switch (currentItem.itemType)
            {
                case ItemType.Commodity :
                    if (currentTile.canDropItem && currentItem.canDropped)
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInvalid();
                    }
                    break;
            }
        }
        else
        {
            SetCursorInvalid();
        }
    }
}
