using System;
using Map.Logic;
using UnityEngine;
using Utility;
namespace Crop.Logic
{
    /// <summary>
    /// 场景里已经存在的物体
    /// </summary>
    public class CropGenerator : MonoBehaviour
    {
        /// <summary>
        /// 当前的Grid
        /// 用来确定位置
        /// </summary>
        private Grid _currenGrid;
        /// <summary>
        /// 格子Id
        /// </summary>
        public int seedItemID;
        /// <summary>
        /// 当前的成长的天数
        /// 预先加载的农作物已经成长几天
        /// </summary>
        public int growthDays;

        private void OnEnable()
        {
            MyEventHandler.GeneratorCropEvent += GeneratorCrop;
        }

        private void OnDisable()
        {
            MyEventHandler.GeneratorCropEvent -= GeneratorCrop;
        }
        private void Awake()
        {
            _currenGrid = FindObjectOfType<Grid>();
        }

        void GeneratorCrop()
        {
            Vector3Int cropGridPos = _currenGrid.WorldToCell(transform.position);
            // 更新地图内容

            if (seedItemID != 0)
            {
                var tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(cropGridPos);
                if (tile == null)
                {
                    tile = new TileDetails();
                    tile.pos = (Vector2Int)cropGridPos;
                }

                tile.daySinceWatered = -1;
                tile.seedItemId = seedItemID;
                tile.growthDays = growthDays;

                GridMapManager.Instance.UpdateTileDetails(tile);
            }
        }
    }
}