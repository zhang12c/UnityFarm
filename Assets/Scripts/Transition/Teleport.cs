using System;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SceneName]
    public string transitionScene;
    public Vector3 posToGo;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            MyEvnetHandler.CallSceneTransitionEvent(transitionScene,posToGo);
        }
    }
}
