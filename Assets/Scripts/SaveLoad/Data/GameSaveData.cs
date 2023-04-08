using System.Collections.Generic;
namespace SaveLoad.Data
{
    /// <summary>
    /// 保存的具体数据
    /// </summary>
    [System.Serializable]
    public class GameSaveData
    {
        /// <summary>
        /// 场景名称
        /// </summary>
        public string dataSceneName;
        /// <summary>
        /// 人物坐标、string 人物名字
        /// </summary>
        public Dictionary<string, SerializableVector3> characterPosDict;
        
        /// <summary>
        /// 丢在场景的道具的数据
        /// </summary>
        public Dictionary<string, List<SceneItemSave>> sceneItemDict;
        
        // 装饰物的数据
        
        /// <summary>
        /// 地图的瓦片信息
        /// </summary>
        public Dictionary<string, TileDetails> tileDetailsDict;
        
        /// <summary>
        /// 地图是否被加载过了
        /// </summary>
        public Dictionary<string, bool> firstLoadDict;
        
        /// <summary>
        /// 背包的数据
        /// </summary>
        public Dictionary<string, List<InventoryItem>> inventoryDict;
        
        /// <summary>
        /// 时间
        /// </summary>
        public Dictionary<string, int> timeDict;
        
        /// <summary>
        /// 钱币
        /// </summary>
        public int playMoney;

        #region NPC相关
        /// <summary>
        /// npc行为表里面要去的scene
        /// </summary>
        public string targetScene;
        /// <summary>
        /// 行为表里是否是可以交互
        /// </summary>
        public bool interactable;
        /// <summary>
        /// 当npc停下来的时候播放的animation clip 的全局唯一ID
        /// </summary>
        public int animationInstanceID;
        #endregion

    }
}