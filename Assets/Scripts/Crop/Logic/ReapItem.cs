using Crop.Data;
using UnityEngine;
using Utility;
namespace Crop.Logic
{
    /// <summary>
    /// 杂草的一些
    /// </summary>
    public class ReapItem : MonoBehaviour
    {
        
        private CropDetails _cropDetails;

        private Transform PlayerTransForm
        {
            get
            {
                return FindObjectOfType<Player.Player>().transform;
            }
        }

        public void InitCropData(int Id)
        {
            _cropDetails = CropManager.Instance.GetCropDetails(Id);
        }

        /// <summary>
        /// 生成农作物
        /// </summary>
        public void SpawnHarvestItems()
        {
            for (int i = 0; i < _cropDetails.producedItemID.Length; i++)
            {
                // 数量
                int amountToProduce;
                if (_cropDetails.producedMaxAmount[i] == _cropDetails.producedMinAmount[i])
                {
                    amountToProduce = _cropDetails.producedMaxAmount[i];
                }
                else
                {
                    amountToProduce = Random.Range(_cropDetails.producedMinAmount[i], _cropDetails.producedMaxAmount[i] + 1);
                }
                
                // 生成数量物品
                for (int j = 0; j < amountToProduce; j++)
                {
                    if (_cropDetails.generateAtPlayerPosition) // 在玩家身上直接生成
                    {
                        MyEventHandler.CallNewAtPlayerPosition(_cropDetails.producedItemID[i]);
                    }
                    else
                    {
                        // 生成到地图上的
                        // 随机一个位置
                        var dirX = transform.position.x > PlayerTransForm.position.x ? 1 : -1;
                        var position = transform.position;
                        var spawPos = new Vector3(position.x + Random.Range(dirX, _cropDetails.spawnRadiuse.x * dirX), //x
                            position.y + Random.Range(Random.Range(-_cropDetails.spawnRadiuse.y, _cropDetails.spawnRadiuse.y), //y
                                0) //z
                        );
                        
                        MyEventHandler.CallCloneCloneSlotInWorld(_cropDetails.producedItemID[i],spawPos);
                    }
                }
            }
        }
    }
}