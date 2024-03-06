using System;
using UnityEngine;
using Utility;

public class Teleport : MonoBehaviour
{
    [SceneName]
    public string transitionScene;
    public Vector3 posToGo;
    private bool _isTriggering;

    private void Awake()
    {
        _isTriggering = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && _isTriggering == false)
        {
            UnityEngine.Debug.Log("OnTriggerEnter2D");
            _isTriggering = true;
            MyEventHandler.CallSceneTransitionEvent(transitionScene,posToGo);
            _isTriggering = false;
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player") && _isTriggering == false )
        {
            UnityEngine.Debug.Log("OnTriggerStay");
            _isTriggering = true;
            MyEventHandler.CallSceneTransitionEvent(transitionScene,posToGo);
            _isTriggering = false;
        }
    }
}
