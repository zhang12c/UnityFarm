using UnityEngine;
using UnityEngine.Serialization;
using Utility;
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
        // 再次生长的事件
        public int daysToRegrow;
        // 重新生长的次数
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
        [FormerlySerializedAs("hasParticalEffect")]
        public bool hasParticleEffect;
        
        // 音效 特效
        public ParticleEffectType particleEffectType;
        /// <summary>
        /// 特效位置
        /// </summary>
        public Vector3 effectPos;

        /// <summary>
        /// 检验tool是否可以当收割道具用
        /// </summary>
        /// <param name="toolID">道具ID</param>
        /// <returns></returns>
        public bool CheckToolAvailable(int toolID)
        {
            foreach (var toolId in harvestToolItemID)
            {
                if (toolId == toolID)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 使用道具几次，才能收割
        /// </summary>
        /// <param name="toolId">道具ID</param>
        /// <returns></returns>
        public int GetTotalRequireCount(int toolId)
        {
            for (int i = 0; i < harvestToolItemID.Length; i++)
            {
                if (harvestToolItemID[i] == toolId)
                {
                    // 潜规则：可使用的道具和可使用的次数一一对应的
                    return requireActionCount[i];
                }
            }
            return -1;
        }
    }
}