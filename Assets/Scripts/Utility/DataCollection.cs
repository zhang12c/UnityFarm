using UnityEngine;
/// <summary>
/// 该文件主要拿来类似枚举
/// </summary>
[System.Serializable]
public class ItemDetails
{
    // BItem
    public int itemID;
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;
    /// <summary>
    /// 物品在地图的图片
    /// </summary>
    public Sprite itemOnWorldSprite;
    public string itemDescription;
    /// <summary>
    /// 道具使用半径
    /// </summary>
    public int itemUseRadius;

    // 道具的状态
    /// <summary>
    /// 捡起
    /// </summary>
    public bool canPickedup;
    /// <summary>
    /// 可丢弃
    /// </summary>
    public bool canDropped;
    /// <summary>
    /// 可举起
    /// </summary>
    public bool canCarried;

    /// <summary>
    /// 出售或购买价格
    /// </summary>
    public int itemPrice;
    /// <summary>
    /// 折扣
    /// </summary>
    [Range(0, 1)]
    public float sellPercentage;
}
[System.Serializable]
public struct InventoryItem
{
    public int itemID;
    public int itemAmount;
}

[System.Serializable]
public class AnimatorType
{
    public PartType partType;
    public PartName partName;
    public AnimatorOverrideController animatorOverrideController;
}

public enum Season
{
    /// <summary>
    /// 春夏秋冬
    /// </summary>
    Spring,Summer,Autumn,Winter
}

/// <summary>
/// 用来存场景物品的位置
/// 序列化 为了保存
/// </summary>
[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;
    public SerializableVector3(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int((int)x, (int)y);
    }
    
}

/// <summary>
/// 场景需要保留的
/// 用这两个结构来保存
/// </summary>
[System.Serializable]
public class SceneItemSave
{
    public int itemID;
    public SerializableVector3 itemSerializableVector3;
}

/// <summary>
/// 单独瓦片类型
/// 基本信息
/// </summary>
[System.Serializable]
public class TileProperty
{
    /// <summary>
    /// 地图格子的坐标
    /// </summary>
    public Vector2Int titleCoordinate;
    /// <summary>
    /// 格子类型
    /// </summary>
    public GridType gridType;
    public bool BoolTypeValue;

}