using System;
using DG.Tweening;
using UnityEngine;
using Utility;

// 一定要有这个组件
[RequireComponent(typeof(SpriteRenderer))]

/// 人物靠近树，就给树做隐藏
public class ItemFader : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void FadeIn()
    {
        //Debug.Log("Item FadeIn");
        Color targetColor = new Color(1, 1, 1, Settings.TARGET_FADE_ALPHA);
        _spriteRenderer.DOColor(targetColor, Settings.FADEIN_TIME);
    }
    public void FadeOut()
    {
        //Debug.Log("Item FadeOut");
        Color targetColor = new Color(1, 1, 1, 1);
        _spriteRenderer.DOColor(targetColor, Settings.FADEIN_TIME);
    }

    /// <summary>
    /// 树木被看倒的事件 在动画的结尾
    /// </summary>
    public void OnAnimatorDone()
    {
        MyEventHandler.CallTreeFallingAnimationOverEven();
    }
}
