using UnityEngine;
/// <summary>
/// 该文件主要拿来类似枚举
/// </summary>
[System.Serializable]
public class ItemDetails
{
    // BItem
    public int itemID;
    public string name;
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
    /// 拿起
    /// </summary>
    public bool canPickup;
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
