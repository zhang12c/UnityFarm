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
    Break
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