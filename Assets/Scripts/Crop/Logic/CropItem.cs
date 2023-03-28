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

        public void ProcessToolAction(ItemDetails tool)
        {
            // 一共需要使用多少次道具
            int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemID);
            if (requireActionCount == -1)
            {
                Debug.Log(tool.itemID + "收集" + cropDetails.seedItemID + " 收集的次数是不是配置错误了？？？");
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
                    if (cropDetails.generateAtPlayerPosition)
                    {
                        MyEventHandler.CallNewAtPlayerPosition(cropDetails.producedItemID[i]);
                    }
                }
            }
        }
    }
}
