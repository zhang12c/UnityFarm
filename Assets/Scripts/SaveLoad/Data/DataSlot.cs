using System.Collections.Generic;
namespace SaveLoad.Data
{
    /// <summary>
    /// 存档 内容
    /// UI 里存档的每一行格子
    /// String GUID
    /// </summary>
    public class DataSlot
    {
        public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();
        
    }
}