using Crop.Data;
using UnityEngine;
using UnityEngine.Serialization;
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
        public TileDetails tileDetails;
        // 收割过程中的动画
        private Animator _animator;
        // 实现左右摇晃
        // 获得人物的坐标 与 种子的坐标
        private Transform PlayerTransForm
        {
            get
            {
                return FindObjectOfType<Player>().transform;
            }
        }

        /// <summary>
        /// 是否已经成熟了，可以收割的
        /// </summary>
        public bool CanHarvest
        {
            get
            {
                return cropDetails.TotalGrowthDays <= tileDetails.growthDays;
            }
        }

        public void ProcessToolAction(ItemDetails tool,TileDetails tileDetails)
        {
            this.tileDetails = tileDetails;
            // 一共需要使用多少次道具
            int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemID);
            if (requireActionCount == -1)
            {
                Debug.LogError(tool.itemID + "收集" + cropDetails.seedItemID + " 收集的次数是不是配置错误了？？？");
                return; 
            }

            // 是否有动画
            _animator = GetComponentInChildren<Animator>();

            // 计算点击的次数
            if (_doActionCount < requireActionCount)
            {
                _doActionCount++;
                Debug.Log($"使用Tool ： {_doActionCount} 次");
                // 声音 + 效果 类似 砍树
                if (_animator != null && cropDetails.hasAnimation)
                {
                    if (PlayerTransForm.position.x < transform.position.x) // 在树的左边
                    {
                        _animator.SetTrigger("RotateRight");
                    }
                    else
                    {
                        _animator.SetTrigger("RotateLeft");
                    }
                }
            }
            else
            {
                Debug.Log("收割成功 逻辑");
                // 收割成功 逻辑
                if (cropDetails.generateAtPlayerPosition) // 直接加到背包中
                {
                    // 生成农作物
                    SpawnHarvestItems();
                }else if (cropDetails.hasAnimation) // 有动画的
                {
                    
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
            if (tileDetails != null)
            {
                // 收获的次数 ++
                tileDetails.daysSinceLastHarvest++;
                
                // 是否重复成长
                if (cropDetails.daysToRegrow > 0 && tileDetails.daysSinceLastHarvest < cropDetails.regrowTimes )
                {
                    // 再次生长将天数降低 当达到TatalGrowthDays的时候又是一次成熟
                    tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.daysToRegrow;
                    // 刷新种子的样子
                    MyEventHandler.CallRefreshCurrentMapEven();
                }
                else
                {
                    // 不可多次收获的
                    // 那就移除种子 初始化格子信息
                    tileDetails.daysSinceLastHarvest = -1;
                    tileDetails.seedItemId = -1;
                    
                }
                Destroy(gameObject);
            }
        }
        //
        
        
        
    }
}
