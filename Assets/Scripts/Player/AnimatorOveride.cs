using System.Collections;
using System.Collections.Generic;
using Inventory.Logic;
using UnityEngine;
using Utility;

public class AnimatorOveride : MonoBehaviour
{
    /// <summary>
    /// player 身上的的所有动画
    /// </summary>
    private Animator[] aimiAnimators;
    /// <summary>
    /// 举起物品的图片
    /// </summary>
    public SpriteRenderer holdItemImage;

    /// <summary>
    /// 各部分动画列表
    /// </summary>
    public List<AnimatorType> animatorTypes;
    
    /// <summary>
    /// 字典 string => animator name
    /// animator
    /// </summary>
    public Dictionary<string, Animator> animatorNameDict = new Dictionary<string, Animator>();

    private void Awake()
    {
        aimiAnimators = GetComponentsInChildren<Animator>();
        foreach (var aimi in aimiAnimators)
        {
            animatorNameDict.Add(aimi.name,aimi);
        }
    }
    private void OnEnable()
    {
        MyEventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        MyEventHandler.NewAtPlayerPositionEvent += OnNewAtPlayerPositionEvent;
    }
    private void OnDisable()
    {
        MyEventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        MyEventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        MyEventHandler.NewAtPlayerPositionEvent -= OnNewAtPlayerPositionEvent;
    }
    
    /// <summary>
    ///  切换场景后将举起的状态回复回去
    /// </summary>
    private void OnAfterSceneLoadEvent()
    {
        holdItemImage.enabled = false;
        SwitchAnimator(PartType.None);
    }
    /// <summary>
    /// 选中了什么道具，播放举起动作，设置举起图片
    /// </summary>
    /// <param name="itemDetails"></param>
    /// <param name="isSelected"></param>
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        //---//
        // 道具的type 与 PartType 对比
        // 种子，商品，等可以被举起
        // TODO: 这里需要补全可以被举起的类型
        PartType partType = itemDetails.itemType switch
        {
            ItemType.seed => PartType.Carry,
            ItemType.Commodity => PartType.Carry,
            ItemType.HoeTool => PartType.Hoe,
            ItemType.WaterTool => PartType.Water,
            ItemType.CollectTool => PartType.Collect,
            _ => PartType.None
        };
        if (!isSelected)
        {
            partType = PartType.None;
            holdItemImage.enabled = false;
        }
        else
        {
            if (partType == PartType.Carry)
            {
                holdItemImage.sprite = itemDetails.itemOnWorldSprite;
                holdItemImage.enabled = true;
            }
            else
            {
                holdItemImage.enabled = false;
                // 把手放下，不然他arm 的不改变
                //SwitchAnimator(PartType.None);
            }
        }
        SwitchAnimator(partType);
    }
    
    // 由 partType 得到 对应的animator
    private void SwitchAnimator(PartType partType)
    {
        foreach (var animatorType in animatorTypes)
        {
            if (animatorType.partType == partType)
            {
                animatorNameDict[animatorType.partName.ToString()].runtimeAnimatorController = animatorType.animatorOverrideController;
            }
        }
    }
    
    /// <summary>
    /// 收获时举起道具显示
    /// </summary>
    /// <param name="id"></param>
    private void OnNewAtPlayerPositionEvent(int id)
    {
        Sprite sprite = InventoryManager.Instance.GetItemDetails(id).itemOnWorldSprite;
        if (holdItemImage.enabled == false)
        {
            StartCoroutine(ShowItem(sprite));
        }
    }

    /// <summary>
    /// 短暂显示一下举着的道具
    /// </summary>
    /// <param name="sprite"></param>
    /// <returns></returns>
    private IEnumerator ShowItem(Sprite sprite)
    {
        holdItemImage.sprite = sprite;
        holdItemImage.enabled = true;
        yield return new WaitForSeconds(1f);
        holdItemImage.enabled = false;
    }
}
