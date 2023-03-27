using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;
using Grid = UnityEngine.Grid;

namespace Map.Logic
{
    public class GridMapManager :  Singleton<GridMapManager>
    {
        [Header("地图瓦片信息List")]
        public List<MapData_SO> mapDataList;

        /// <summary>
        /// 不同地图不通坐标下的瓦片信息是什么呢
        /// key: mapName+坐标 value: TitleDetails
        /// </summary>
        private Dictionary<string, TileDetails> tileDetailsMap = new Dictionary<string, TileDetails>();

        private Grid currentGrid;

        private void Start()
        {
            // 构建一下数据
            foreach (var mapData in mapDataList)
            {
                InitTileDetailsDict(mapData);
            }
        }

        private void OnEnable()
        {
            MyEventHandler.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        }

        private void OnDisable()
        {
            MyEventHandler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
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
                    tileDetailsMap[st.ToString()] = tileDetails;
                }
                else
                {
                    tileDetailsMap.Add(st.ToString(), tileDetails);
                }
            
            
            }
        }

        /// <summary>
        /// 用Key 来返回 瓦片信息
        /// </summary>
        /// <param name="key">x + y + 地图名称</param>
        /// <returns></returns>
        private TileDetails GetTileDetails(string key)
        {
            if (tileDetailsMap.ContainsKey(key))
            {
                return tileDetailsMap[key];
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
            currentGrid = FindObjectOfType<Grid>();
        }
    
        /// <summary>
        /// 在玩家播放完动画之后
        /// 需要对数据做修改了
        /// </summary>
        /// <param name="mouseWorldPos">鼠标世界坐标</param>
        /// <param name="itemdetails">选中的道具ItemDetails</param>
        private void OnExecuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetails itemdetails)
        {
            Vector3Int mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
            TileDetails currentTile = GetTileDetailsOnMousePosition(mouseGridPos);
            if (currentTile != null)
            {
                switch (itemdetails.itemType)
                {
                    case ItemType.Commodity:
                        MyEventHandler.CallDropItemEvent(itemdetails.itemID,mouseWorldPos);
                        break;
                }
            }
        }
    }
}
