using System.Collections.Generic;
using System.Text;
using Crop.Logic;
using SaveLoad.Data;
using SaveLoad.Logic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Utility;
using Grid = UnityEngine.Grid;
using Random = UnityEngine.Random;

namespace Map.Logic
{
    public class GridMapManager :  Singleton<GridMapManager> ,ISaveAble
    {
        [Header("种地瓦片")]
        public RuleTile digTile;
        public RuleTile waterTile;

        private Tilemap _digTileMap;
        private Tilemap _waterTileMap;
        
        [Header("地图瓦片信息List")]
        public List<MapData_SO> mapDataList;

        /// <summary>
        /// 当前的季节
        /// 以后用来判断种子可否种下
        /// </summary>
        private Season _season;

        /// <summary>
        /// 不同地图不通坐标下的瓦片信息是什么呢
        /// key: mapName+坐标 value: TitleDetails
        /// </summary>
        private Dictionary<string, TileDetails> _tileDetailsMap = new Dictionary<string, TileDetails>();

        private Grid _currentGrid;

        /// <summary>
        /// 主要用于判断当下场景是不是第一次加载
        /// string 名称
        /// bool 
        /// </summary>
        private Dictionary<string, bool> _firstLoadDict = new Dictionary<string, bool>();
        
        /// <summary>
        /// 杂草的list
        /// </summary>
        private List<ReapItem> _reapItems;
        private string _guid;

        private void Start()
        {
            ISaveAble saveAble = this;
            saveAble.RegisterSaveAble();
            
            // 构建一下数据
            foreach (var mapData in mapDataList)
            {
                _firstLoadDict.Add(mapData.sceneName,true);
                //
                InitTileDetailsDict(mapData);
            }
        }

        private void OnEnable()
        {
            MyEventHandler.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            MyEventHandler.GameDayEvent += OnGameDayEvent;
            MyEventHandler.RefreshCurrentMapEven += RefreshMap;
        }

        private void OnDisable()
        {
            MyEventHandler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            MyEventHandler.GameDayEvent -= OnGameDayEvent;
            MyEventHandler.RefreshCurrentMapEven -= RefreshMap;

        }
        
        /// <summary>
        /// 初始化tile瓦片字典
        /// </summary>
        /// <param name="mapDataSo"></param>
        private void InitTileDetailsDict(MapData_SO mapDataSo)
        {
            foreach (TileProperty tileProperties in mapDataSo.titleProperties)
            {
                TileDetails tileDetails = new TileDetails()
                {
                    pos = new Vector2Int(tileProperties.titleCoordinate.x, tileProperties.titleCoordinate.y),
                };
                // 字典的Key
                StringBuilder st = new StringBuilder();
                st.Append(tileDetails.pos.x + "x" + tileDetails.pos.y + "y" + mapDataSo.sceneName);

                if (GetTileDetails(st.ToString()) != null)
                {
                    tileDetails = GetTileDetails(st.ToString());
                }

                switch (tileProperties.gridType)
                {
                    case GridType.Dig:
                        tileDetails.canDig = tileProperties.BoolTypeValue;
                        break;
                    case GridType.DropItem :
                        tileDetails.canDropItem = tileProperties.BoolTypeValue;
                        break;
                    case GridType.PlaceFurniture :
                        tileDetails.canPlaceFurniture = tileProperties.BoolTypeValue;
                        break;
                    case GridType.NPCObstacle :
                        tileDetails.isNPCObstacle = tileProperties.BoolTypeValue;
                        break;
                }
                if (GetTileDetails(st.ToString()) != null)
                {
                    _tileDetailsMap[st.ToString()] = tileDetails;
                }
                else
                {
                    _tileDetailsMap.Add(st.ToString(), tileDetails);
                }
            
            
            }
        }

        /// <summary>
        /// 用Key 来返回 瓦片信息
        /// </summary>
        /// <param name="key">x + y + 地图名称</param>
        /// <returns></returns>
        public TileDetails GetTileDetails(string key)
        {
            if (_tileDetailsMap.ContainsKey(key))
            {
                return _tileDetailsMap[key];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据网格坐标获得网格信息
        /// </summary>
        /// <param name="mouseGridPos"></param>
        /// <returns></returns>
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos)
        {
            string key = mouseGridPos.x + "x" + mouseGridPos.y + "y" + SceneManager.GetActiveScene().name;
            return GetTileDetails(key);
        }
    
        private void OnAfterSceneLoadEvent()
        {
            _currentGrid = FindObjectOfType<Grid>();
            _digTileMap = GameObject.FindWithTag("Dig")?.GetComponent<Tilemap>();
            _waterTileMap = GameObject.FindWithTag("Water")?.GetComponent<Tilemap>();

            // 主要目的是：第一次地图上的手动按的树不会被销毁
            if (_firstLoadDict[SceneManager.GetActiveScene().name])
            {
                // 第一次加载
                // 更新deteils
                MyEventHandler.CallGeneratorCropEvent();
                _firstLoadDict[SceneManager.GetActiveScene().name] = false;
            }
            
            // 刷新地图了
            RefreshMap();
        }
        /// <summary>
        /// 每一天执行一次
        /// </summary>
        /// <param name="day"></param>
        /// <param name="season"></param>
        private void OnGameDayEvent(int day, Season season)
        {
            // 保存一下当前的季节
            _season = season;
            
            // 刷新tile 的日期
            foreach (var tile in _tileDetailsMap)
            {
                if (tile.Value.daySinceWatered > -1)
                {
                    tile.Value.daySinceWatered = -1;
                }
                if (tile.Value.daySinceDug > -1)
                {
                    tile.Value.daySinceDug++;
                }
                if (tile.Value.daySinceDug > 3 && tile.Value.seedItemId == -1 && Random.Range(1,5) >= 2)
                {
                    tile.Value.daySinceDug = -1;
                    tile.Value.canDig = true;
                    tile.Value.growthDays = -1;
                }
                if (tile.Value.seedItemId != -1)
                {
                    tile.Value.growthDays++;
                }
            }
            RefreshMap();
        }
    
        /// <summary>
        /// 在玩家播放完动画之后
        /// 需要对数据做修改了
        /// </summary>
        /// <param name="mouseWorldPos">鼠标世界坐标</param>
        /// <param name="itemdetails">选中的道具ItemDetails</param>
        private void OnExecuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetails itemdetails)
        {
            Vector3Int mouseGridPos = _currentGrid.WorldToCell(mouseWorldPos);
            TileDetails currentTile = GetTileDetailsOnMousePosition(mouseGridPos);
            if (currentTile != null)
            {
                CropItem cropItem;
                switch (itemdetails.itemType)
                {
                    // 绘Tile
                    case ItemType.seed:
                        MyEventHandler.CallPlantSeedEvent(itemdetails.itemID, currentTile);
                        MyEventHandler.CallDropItemEvent(itemdetails.itemID,mouseWorldPos,itemdetails.itemType);
                        MyEventHandler.CallPlaySoundEvent(SoundName.Plant);
                        break;
                    case ItemType.Commodity:
                        MyEventHandler.CallDropItemEvent(itemdetails.itemID,mouseWorldPos,itemdetails.itemType);
                        break;
                    case ItemType.HoeTool:
                        SetDigGround(currentTile);
                        currentTile.daySinceDug = 0;
                        currentTile.canDig = false;
                        currentTile.canDropItem = false;
                        // TODO: 翻土音效
                        MyEventHandler.CallPlaySoundEvent(SoundName.Hoe);

                        break;
                    case ItemType.WaterTool:
                        SetWaterGround(currentTile);
                        currentTile.daySinceWatered = 0;
                        // TODO: 浇水音效
                        MyEventHandler.CallPlaySoundEvent(SoundName.Water);

                        break;
                    case ItemType.BreakTool:
                    case ItemType.ChopTool: // 斧头 收割
                        cropItem = GetCropObject(mouseGridPos);
                        if (cropItem != null)
                        {
                            cropItem.ProcessToolAction(itemdetails,cropItem.tileDetails);
                        }
                        else
                        {
                            Debug.Log("没Tool Do ");
                        }
                        break;
                    case ItemType.CollectTool:
                        cropItem = GetCropObject(mouseWorldPos);
                        if (cropItem != null)
                        {
                            // 收割逻辑写在cropItem里面
                            cropItem.ProcessToolAction(itemdetails,currentTile);
                        }
                        break;
                    case ItemType.ReapTool:
                        int count = 0;
                        for (int i = 0; i < _reapItems.Count; i++)
                        {
                            MyEventHandler.CallParticleEffectEvent(ParticleEffectType.ReapableScenery,_reapItems[i].transform.position + Vector3.up);
                            _reapItems[i].SpawnHarvestItems();
                            Destroy(_reapItems[i].gameObject);
                            count++;
                            if (count > Settings.REAP_MAX_COUNT)
                            {
                                break;
                            }
                        }
                        break;
                }

                UpdateTileDetails(currentTile);
            }
        }
        /// <summary>
        /// 挖土的位置绘制泥土 RuleTile
        /// 显示挖土瓦片
        /// </summary>
        /// <param name="tileDetails"></param>
        private void SetDigGround(TileDetails tileDetails)
        {
            Vector3Int pos = new Vector3Int(tileDetails.pos.x, tileDetails.pos.y,0);
            if (_digTileMap != null)
            {
                _digTileMap.SetTile(pos,digTile);
            }
        }

        private void SetWaterGround(TileDetails tileDetails)
        {
            Vector3Int pos = new Vector3Int(tileDetails.pos.x, tileDetails.pos.y,0);
            if (_waterTileMap != null)
            {
                _waterTileMap.SetTile(pos,waterTile);
            }
        }
        /// <summary>
        /// 更新瓦片信息
        /// </summary>
        /// <param name="tileDetails"></param>
        public void UpdateTileDetails(TileDetails tileDetails)
        {
            string key = tileDetails.pos.x + "x" + tileDetails.pos.y + "y" + SceneManager.GetActiveScene().name;
            if (_tileDetailsMap.ContainsKey(key))
            {
                _tileDetailsMap[key] = tileDetails;
            }
            else
            {
                _tileDetailsMap.Add(key,tileDetails);
            }
        }

        /// <summary>
        /// 重新绘制一下地图
        /// </summary>
        private void RefreshMap()
        {
            if (_digTileMap != null)
            {
                _digTileMap.ClearAllTiles();
            }
            if (_waterTileMap != null)
            {
                _waterTileMap.ClearAllTiles();
            }
            
            // 清空一下原有的作物
            foreach (var cropItem in FindObjectsOfType<CropItem>())
            {
                Destroy(cropItem.gameObject);
            }
            
            
            ShowTileMap(SceneManager.GetActiveScene().name);
        }
        /// <summary>
        /// 显示tile信息
        /// 主要在切换场景的时候执行
        /// </summary>
        /// <param name="sceneName"></param>
        private void ShowTileMap(string sceneName)
        {
            foreach (var tile in _tileDetailsMap)
            {
                var key = tile.Key;
                var tileDetails = tile.Value;
                if (key.Contains(sceneName))
                {
                    if (tileDetails.daySinceDug > -1)
                    {
                        SetDigGround(tileDetails);
                    }
                    if (tileDetails.daySinceWatered > -1)
                    {
                        SetWaterGround(tileDetails);
                    }
                    if (tileDetails.seedItemId > -1)
                    {
                        MyEventHandler.CallPlantSeedEvent(tileDetails.seedItemId,tileDetails);
                    }
                }
            }
        }

        public CropItem GetCropObject(Vector3 mouseWorldPos)
        {
            Collider2D[] collider2Ds = Physics2D.OverlapPointAll(mouseWorldPos);
            for (int i = 0; i < collider2Ds.Length; i++)
            {
                if (collider2Ds[i].GetComponent<CropItem>() != null)
                {
                    return collider2Ds[i].GetComponent<CropItem>();
                }
            }
            return null;
        }
        /// <summary>
        /// 检测鼠标的范围的ReadItem
        /// </summary>
        /// <param name="itemDetails"></param>
        /// <returns>有数据就true</returns>
        public bool HaveReapItemsInRadius(ItemDetails itemDetails,Vector3 mouseWorldPos)
        {
            _reapItems = new List<ReapItem>();
            Collider2D[] collider2Ds = new Collider2D[10];
            Physics2D.OverlapCircleNonAlloc(mouseWorldPos,itemDetails.itemUseRadius, collider2Ds);
            if (collider2Ds.Length > 0)
            {
                for (int i = 0; i < collider2Ds.Length; i++)
                {
                    if (collider2Ds[i]?.GetComponent<ReapItem>())
                    {
                        var item = collider2Ds[i].GetComponent<ReapItem>();
                        _reapItems.Add(item);
                    }
                }
            }
            return _reapItems.Count > 0;
        }

        /// <summary>
        /// A星算法 获得地图覆盖的范围
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="gridDimensions"></param>
        /// <param name="gridOrigin"></param>
        /// <returns></returns>
        public bool GetGridDimensions(string sceneName,out Vector2Int gridDimensions,out Vector2Int gridOrigin)
        {
            gridDimensions = Vector2Int.one;
            gridOrigin = Vector2Int.one;
            foreach (MapData_SO mapDataSo in mapDataList)
            {
                if (mapDataSo.sceneName == sceneName)
                {
                    gridDimensions = new Vector2Int(mapDataSo.gridWidth, mapDataSo.gridHeight);
                    gridOrigin = new Vector2Int(mapDataSo.originX, mapDataSo.originY);
                    return true;
                }
            }
            Debug.LogError($"{sceneName} 没有找到哦！");
            return false;
        }
        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData()
            {
                tileDetailsDict = _tileDetailsMap,
                firstLoadDict = _firstLoadDict,
            };
            return saveData;
        }
        public void RestoreData(GameSaveData saveData)
        {
            _firstLoadDict = saveData.firstLoadDict;
            _tileDetailsMap = saveData.tileDetailsDict;
        }
        string ISaveAble.GUID
        {
            get
            {
                return GetComponent<DataGUID>().guid;
            }
        }
    }
}
