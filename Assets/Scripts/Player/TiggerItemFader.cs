using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 碰撞就触发物品的透明度
/// </summary>
public class TiggerItemFader : MonoBehaviour
{
    /// <summary>
    /// 注意这里方法是触发器，而不是碰撞器
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerEnter2D(Collider2D col)
    {
        ItemFader[] faders = col.gameObject.GetComponentsInChildren<ItemFader>();
        if (faders.Length > 0)
        {
            foreach (var fader in faders)
            {
                fader.FadeIn();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ItemFader[] faders = other.gameObject.GetComponentsInChildren<ItemFader>();
        if (faders.Length > 0)
        {
            foreach (var fader in faders)
            {
                fader.FadeOut();
            }
        }
    }
}
