using Crop.Data;
using UnityEngine;
using Utility;
namespace Crop.Logic
{
    public class CropItem : MonoBehaviour
    {
        // 收割的逻辑
        public CropDetails cropDetails;
        // 点击的次数
        private int _doActionCount = 0;
        // 
        private TileDetails _tileDetails;

        public void ProcessToolAction(ItemDetails tool,TileDetails tileDetails)
        {
            _tileDetails = tileDetails;
            // 一共需要使用多少次道具
            int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemID);
            if (requireActionCount == -1)
            {
                Debug.LogError(tool.itemID + "收集" + cropDetails.seedItemID + " 收集的次数是不是配置错误了？？？");
                return; 
            }

            // 是否有动画
            
            // 计算点击的次数
            if (_doActionCount < requireActionCount)
            {
                _doActionCount++;
                // 声音 + 效果
            }
            else
            {
                if (cropDetails.generateAtPlayerPosition)
                {
                    // 生成农作物
                    SpawnHarvestItems();
                }
            }


        }

        public void SpawnHarvestItems()
        {
            for (int i = 0; i < cropDetails.producedItemID.Length; i++)
            {
                // 数量
                int amountToProduce;
                if (cropDetails.producedMaxAmount[i] == cropDetails.producedMinAmount[i])
                {
                    amountToProduce = cropDetails.producedMaxAmount[i];
                }
                else
                {
                    amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i] + 1);
                }
                
                // 生成数量物品
                for (int j = 0; j < amountToProduce; j++)
                {
                    if (cropDetails.generateAtPlayerPosition) // 在玩家身上直接生成
                    {
                        MyEventHandler.CallNewAtPlayerPosition(cropDetails.producedItemID[i]);
                    }
                    else
                    {
                        // 生成到地图上的
                        
                    }
                }
            }
            if (_tileDetails != null)
            {
                // 收获的次数 ++
                _tileDetails.daysSinceLastHarvest++;
                
                // 是否重复成长
                if (cropDetails.daysToRegrow > 0 && _tileDetails.daysSinceLastHarvest < cropDetails.regrowTimes )
                {
                    // 再次生长将天数降低 当达到TatalGrowthDays的时候又是一次成熟
                    _tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.daysToRegrow;
                    // 刷新种子的样子
                    MyEventHandler.CallRefreshCurrentMapEven();
                }
                else
                {
                    // 不可多次收获的
                    // 那就移除种子 初始化格子信息
                    _tileDetails.daysSinceLastHarvest = -1;
                    _tileDetails.seedItemId = -1;
                    
                }
                Destroy(gameObject);
            }
        }
        //
        
        
        
    }
}
