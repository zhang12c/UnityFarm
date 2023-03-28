using System.Collections.Generic;
using Crop.Data;
using UnityEngine;
namespace Crop
{
    [CreateAssetMenu(fileName = "CropDataListSo",menuName = "Crop/CropDataListSo")]
    public class CropDataListSo : ScriptableObject
    {
        public List<CropDetails> cropDataList;
    }
}