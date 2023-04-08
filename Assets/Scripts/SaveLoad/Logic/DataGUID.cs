using UnityEngine;
namespace SaveLoad.Logic
{
    /// <summary>
    /// 一直运行的
    /// </summary>
    [ExecuteAlways]
    public class DataGUID : MonoBehaviour
    {
        public string guid;
        private void Awake()
        {
            if (guid == string.Empty)
            {
                // 生成一个16位的guid字符串
                guid = System.Guid.NewGuid().ToString();
            }
        }
    }
}