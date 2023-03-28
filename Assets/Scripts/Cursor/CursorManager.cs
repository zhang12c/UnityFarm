using Crop.Data;
using Crop.Logic;
using Map.Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;
namespace Cursor
{
    public class CursorManager : MonoBehaviour
    {
        /// <summary>
        /// 3中不同状态的鼠标图片
        /// </summary>
        public Sprite normal, tool, seed, item;

        private Sprite _currentSprite;
        private Image _cursorImage;
        private RectTransform _cursorCanvas;
    
        // 鼠标检测 
        // 鼠标滑动到某一块地上
        // 自动识别出这块地能拿来做啥
        private Camera _mainCamera;
        private Grid _currentGrid;
        private Vector3 _mouseWorldPos;
        private Vector3Int _mouseGirdPos;
        /// <summary>
        /// 鼠标的可用性
        /// </summary>
        private bool _cursorEnable = false;
        /// <summary>
        /// 鼠标在这个位置上是否是可用的
        /// </summary>
        private bool _cursorPositionValid = false;
        /// <summary>
        /// 当前选中的道具
        /// </summary>
        private ItemDetails _currentItem;

        private static Transform playerTransform
        {
            get
            {
                return FindObjectOfType<Player>().transform;
            }
        }
        private void OnEnable()
        {
            MyEventHandler.ItemSelectedEvent += OnItemSelectedEvent;
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            MyEventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        }
        private void Start()
        {
            _cursorCanvas = GameObject.FindWithTag("CursorCanvas").GetComponent<RectTransform>();
            _cursorImage = _cursorCanvas.Find("CursorImage")?.GetComponent<Image>();
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (_cursorCanvas != null)
            {
                _cursorImage.transform.position = Input.mousePosition;
                if (!InteractWithUI()&& _cursorEnable)
                {
                    SetCursorImage(_currentSprite);
                    CheckCursorValid();
                    CheckPlayerInput();
                }
                else
                {
                    SetCursorImage(normal);
                }
            }
        }
        /// <summary>
        /// 检测点击左键还是右键
        /// </summary>
        private void CheckPlayerInput()
        {
            if (Input.GetMouseButtonDown(0) && _cursorPositionValid)
            {
                // 执行什么
                MyEventHandler.CallMouseClickedEvent(_mouseWorldPos,_currentItem);
            }
        }

        private void OnDisable()
        {
            MyEventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
            MyEventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            MyEventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;

        }
        private void OnAfterSceneLoadEvent()
        {
            _currentGrid = FindObjectOfType<Grid>();
        }
    
        private void OnBeforeSceneUnloadEvent()
        {
            _cursorEnable = false;
        }

        /// <summary>
        /// 设置鼠标的样式
        /// </summary>
        /// <param name="itemDetails"></param>
        /// <param name="isSelected"></param>
        private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
        {
            if (!isSelected)
            {
                _currentSprite = normal;
                _currentItem = null;
                _cursorEnable = false;
                return;
            }
            _currentSprite = itemDetails.itemType switch
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

            _currentItem = itemDetails;
            _cursorEnable = true;

        }

        /// <summary>
        /// 设置鼠标的图标
        /// </summary>
        /// <param name="sprite"></param>
        private void SetCursorImage(Sprite sprite)
        {
            _cursorImage.sprite = sprite;
            _cursorImage.color = new Color(1, 1, 1);
        }

    #region 鼠标的样式设置
        /// <summary>
        /// 鼠标的可用
        /// 正常显示
        /// </summary>
        private void SetCursorValid()
        {
            _cursorImage.color = new Color(1, 1, 1, 1);
            _cursorPositionValid = true;
        }
    
        /// <summary>
        /// 鼠标不可用
        /// 变红色，变半透明
        /// </summary>
        private void SetCursorInvalid()
        {
            _cursorImage.color = new Color(1, 0, 0, 0.5f);
            _cursorPositionValid = false;
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
            _mouseWorldPos = _mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,-_mainCamera.transform.position.z));
            _mouseGirdPos = _currentGrid.WorldToCell(_mouseWorldPos);
        
            // 道具范围逻辑
            var playerPos = _currentGrid.WorldToCell(playerTransform.position);
            // 如果鼠标超出了道具的使用范围，就直接不可用了
            if (Mathf.Abs(_mouseGirdPos.x - playerPos.x) > _currentItem.itemUseRadius || Mathf.Abs(_mouseGirdPos.y - playerPos.y) > _currentItem.itemUseRadius)
            {
                SetCursorInvalid();
                return;
            }

            var currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(_mouseGirdPos);
            if (currentTile != null)
            {
                CropDetails cropDetails = CropManager.Instance.GetCropDetails(currentTile.seedItemId);
                //TODO: 补充物品类型
                switch (_currentItem.itemType)
                {
                    case ItemType.seed:
                        if (currentTile.daySinceDug > -1 && currentTile.seedItemId == -1)
                            SetCursorValid();
                        else
                            SetCursorInvalid();
                        break;
                    case ItemType.Commodity :
                        if (currentTile.canDropItem && _currentItem.canDropped)
                            SetCursorValid();
                        else
                            SetCursorInvalid();
                        break;
                    case ItemType.HoeTool: // 锄头
                        if (currentTile.canDig)
                            SetCursorValid();
                        else
                            SetCursorInvalid();
                        break;
                    case ItemType.WaterTool: // 水桶
                        if (currentTile.daySinceDug > -1 && currentTile.daySinceWatered == -1 )
                            SetCursorValid();
                        else
                            SetCursorInvalid();
                        break;
                    case ItemType.CollectTool: // 收割
                        if (cropDetails != null)
                            if (currentTile.growthDays >= cropDetails.TotalGrowthDays && cropDetails.CheckToolAvailable(_currentItem.itemID))
                                SetCursorValid();
                            else
                                SetCursorInvalid();
                        else 
                            SetCursorInvalid();
                        break;
                }
            }
            else
            {
                SetCursorInvalid();
            }
        }
    }
}
