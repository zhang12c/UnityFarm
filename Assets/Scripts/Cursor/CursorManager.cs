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
    private Vector3 mouseGirdPos;
    /// <summary>
    /// 鼠标的可用性
    /// </summary>
    private bool cursorEnable = false;
    
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
        cursorEnable = true;
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

    /// <summary>
    /// 需要实时检测鼠标的坐标
    /// 在Update 中执行
    /// </summary>
    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,0));
        mouseGirdPos = currentGrid.WorldToCell(mouseWorldPos);
        Debug.Log($"{mouseWorldPos} : {mouseGirdPos}");
    }
}
