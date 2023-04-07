namespace Utility
{
    public enum ItemType
    {
        /// <summary>
        /// 种子
        /// </summary>
        seed,
        /// <summary>
        /// 商品
        /// </summary>
        Commodity,
        /// <summary>
        /// 家具
        /// </summary>
        Furniture,
        /// <summary>
        /// 锄头
        /// </summary>
        HoeTool,
        /// <summary>
        /// 斧头
        /// </summary>
        ChopTool,
        /// <summary>
        /// 铁镐
        /// </summary>
        BreakTool,
        /// <summary>
        /// 镰刀
        /// </summary>
        ReapTool,
        /// <summary>
        /// 浇水桶
        /// </summary>
        WaterTool,
        /// <summary>
        /// 收集篮
        /// </summary>
        CollectTool,
        /// <summary>
        /// 镰刀
        /// </summary>
        ReapableScenery,
    }

    public enum SlotType
    {
        Bag,
        Box,
        Shop,
    }

    public enum InventoryLocation
    {
        Player,Box
    }

// 我要做什么？
// 不同部位有不同的animation
// 用一个字典装

    /// <summary>
    /// 状态
    /// </summary>
    public enum PartType
    {
        None,
        Carry,
        Hoe,
        Break,
        Water,
        Collect,
        Chop,
        Reap,
    }

    /// <summary>
    /// 部位
    /// </summary>
    public enum PartName
    {
        Body,
        Hair,
        Arm,
        Tool
    }

    /// <summary>
    /// 格子的地图类型
    /// </summary>
    public enum GridType
    {
        /// <summary>
        /// 可以挖洞
        /// </summary>
        Dig,
        /// <summary>
        /// 掉落物品
        /// </summary>
        DropItem,
        /// <summary>
        /// 放置家具
        /// </summary>
        PlaceFurniture,
        /// <summary>
        /// 障碍物
        /// </summary>
        NPCObstacle
    }

    public enum ParticleEffectType
    {
        None,
        LeavesFalling01,
        LeavesFalling02,
        Rock,
        ReapableScenery
    }

    public enum LightShift
    {
        Morning,Night
    }

    public enum SoundName
    {
        None,
        /// <summary>
        /// 走路
        /// </summary>
        FootStepSoft,
        FootStepHard,
#region 工具
        Axe,
        Pickaxe,
        Hoe,
        Reap,
        Water,
        Basket,
        Chop,
#endregion
        /// <summary>
        /// 采摘
        /// </summary>
        Pickup,
        /// <summary>
        /// 种植
        /// </summary>
        Plant,
        /// <summary>
        /// 树倒下
        /// </summary>
        TreeFalling,
        /// <summary>
        /// 割草
        /// </summary>
        Rustle,
        /// <summary>
        /// 环境音乐
        /// </summary>
        AmbientCountryside1,
        AmbientCountryside2,
        /// <summary>
        /// 背景音乐
        /// </summary>
        MusicCalm1,
        MusicCalm3,
        AmbientIndoor1,
    }

    public enum GameState
    {
        Pause,
        Play,
    }
}