using System;
using Cinemachine;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{
    private CinemachineConfiner ccr;
    private PolygonCollider2D _polygonCollider2D;

    private void Awake()
    {
        ccr = GetComponent<CinemachineConfiner>();
    }

    private void SwitchConfinerShape()
     {
         _polygonCollider2D = GameObject.FindWithTag("BoundsConfiner")?.GetComponent<PolygonCollider2D>();
         if (_polygonCollider2D)
         {
             ccr.m_BoundingShape2D = _polygonCollider2D;
             // 切换参加的时候可能边界失效，要用这个方法清一下缓存
             ccr.InvalidatePathCache();
         }
         else
         {
             Debug.LogError("获得边界Bounds失败了");
         }
     }

    private void OnEnable()
    {
        MyEvnetHandler.AfterSceneLoadEvent += OnSceneLoad;
    }
    private void OnDisable()
    {
        MyEvnetHandler.AfterSceneLoadEvent -= OnSceneLoad;
    }
    
    private void OnSceneLoad()
    {
        SwitchConfinerShape();
    }
}
