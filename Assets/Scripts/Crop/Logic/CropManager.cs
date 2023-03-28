using System;
using Crop.Data;
using Inventory.Item;
using UnityEngine;
using Utility;
namespace Crop.Logic
{
    public class CropManager : MonoBehaviour
    {
        public CropDataListSo cropDataListSo;
        /// <summary>
        /// crop 都生成在这个子节点下
        /// </summary>
        private Transform _cropParent;
        /// <summary>
        /// 当前的网格
        /// </summary>
        private Grid _currentGrid;

        private Season _currentSeason;

        private void OnEnable()
        {
            MyEventHandler.PlantSeedEvent += OnPlantSeedEvent;
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            MyEventHandler.GameDayEvent += OnGameDayEvent;
        }

        private void OnDisable()
        {
            MyEventHandler.PlantSeedEvent -= OnPlantSeedEvent;
            MyEventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            MyEventHandler.GameDayEvent -= OnGameDayEvent;
        }
        private void OnGameDayEvent(int day, Season season)
        {
            _currentSeason = season;
        }
        private void OnAfterSceneLoadEvent()
        {
            _currentGrid = FindObjectOfType<Grid>();
            _cropParent = GameObject.FindWithTag("CropParent").transform;
        }
        private void OnPlantSeedEvent(int id, TileDetails tileDetails)
        {
            CropDetails cropDetails = GetCropDetails(id);
            if (cropDetails != null && SeasonAvailable(cropDetails) && tileDetails.seedItemId == -1)
            {
                tileDetails.seedItemId = id;
                tileDetails.growthDays = 0;
                // 显示作物
                DisplayCropPlant(tileDetails,cropDetails);
            }
            else if (tileDetails.seedItemId != -1)
            {
                // 有农作物的
                // 显示作物
                // 刷新一下
                DisplayCropPlant(tileDetails,cropDetails);
            }
        }

        /// <summary>
        /// 生成一下
        /// </summary>
        /// <param name="tileDetails"></param>
        /// <param name="cropDetails"></param>
        private void DisplayCropPlant(TileDetails tileDetails, CropDetails cropDetails)
        {
            // 成长阶段
            int growthStages = cropDetails.growthDays.Length;
            int currentStage = 0;
            int dayCounter = cropDetails.TotalGrowthDays;
            
            // 倒叙 巧妙的逻辑 😁
            for (int i = growthStages - 1; i >= 0; i--)
            {
                if (tileDetails.growthDays >= dayCounter)
                {
                    // 打通关了
                    currentStage = i;
                    break;
                }
                dayCounter -= cropDetails.growthDays[i];
            }
            
            // 获取当前阶段的Prefab
            GameObject currenPrefab = cropDetails.growthPreFab[currentStage];
            // 获取当前阶段的图片
            Sprite currentSprite = cropDetails.growthSprites[currentStage];
            // 瓦片的XY 左下角的点
            Vector3 pos = new Vector3(tileDetails.pos.x + 0.5f, tileDetails.pos.y + 0.5f, 0);
            GameObject cropInstance = Instantiate(currenPrefab,pos, Quaternion.identity,_cropParent);

            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = currentSprite;

        }

        private CropDetails GetCropDetails(int id)
        {
            return cropDataListSo.cropDataList.Find(details => id == details.seedItemID );
        }

        /// <summary>
        /// 判断当前的季节是否可以种植
        /// </summary>
        /// <param name="cropDetails">种子的详情</param>
        /// <returns></returns>
        private bool SeasonAvailable(CropDetails cropDetails)
        {
            for (int i = 0; i < cropDetails.seasons.Length; i++)
            {
                if(cropDetails.seasons[i] == _currentSeason)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
