using UnityEngine;
namespace Crop.Data
{
    [System.Serializable]
    public class CropDetails
    {
        public int seedItemID;
        // 阶段
        [Header("不同阶段需要的天数")]
        public int[] growthDays;
        // 
        public int TotalGrowthDays
        {
            get
            {
                int amount = 0;
                foreach (int day in growthDays)
                {
                    amount += day;
                }
                return amount;
            }
        }
        [Header("不同阶段的物体prefab")]
        public GameObject[] growthPreFab;
        //
        [Header("不同阶段的图片")]
        public Sprite[] growthSprites;
        //
        public Season[] seasons;
        //
        [Space]
        [Header("收割工具")]
        public int[] harvestToolItemID;
        //
        [Header("需要使用次数")]
        public int[] requireActionCount;
        //
        [Header("转变成新物品ID")]
        public int transFerItemID;
        //
        [Space]
        [Header("收获果实信息")]
        public int[] producedItemID;
        public int[] producedMinAmount;
        public int[] producedMaxAmount;
        public Vector2 spawnRadiuse;
        //
        [Header("再次生长时间")]
        public int daysToRegrow;
        public int regrowTimes;
        //
        /// <summary>
        /// 在玩家身上生成？
        /// </summary>
        [Header("Options")]
        public bool generateAtPlayerPosition;
        /// <summary>
        /// 动画
        /// </summary>
        public bool hasAnimation;
        /// <summary>
        /// 粒子特效
        /// </summary>
        public bool hasParticalEffect;


    }
}