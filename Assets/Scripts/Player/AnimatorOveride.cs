using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        MyEvnetHandler.ItemSelectedEvent += OnItemSelectedEvent;
        MyEvnetHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
    }
    private void OnDisable()
    {
        MyEvnetHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        MyEvnetHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
    }
    
    /// <summary>
    ///  切换场景后将举起的状态回复回去
    /// </summary>
    private void OnAfterSceneLoadEvent()
    {
        holdItemImage.enabled = false;
        SwitchAnimator(PartType.None);
    }
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (!isSelected)
        {
            SwitchAnimator(PartType.None);
            holdItemImage.enabled = false;
            return;
        }
        holdItemImage.sprite = itemDetails.itemIcon;
        holdItemImage.enabled = true;
        // 又是一个无语的语法糖
        //---//
        // 道具的type 与 PartType 对比
        // 种子，商品，等可以被举起
        // TODO: 这里需要补全可以被举起的类型
        PartType partType = itemDetails.itemType switch
        {
            ItemType.seed => PartType.Carry,
            ItemType.Commodity => PartType.Carry,
            _ => PartType.None
        };
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
}
