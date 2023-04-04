using System;
using UnityEngine;
using Utility;
namespace Light.Logic
{
    public class LightManager : MonoBehaviour
    {
        private LightController[] sceneLights;

        private LightShift currentLightShift;

        private Season _season;

        private void OnEnable()
        {
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        }
        private void OnDisable()
        {
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        }
        private void OnAfterSceneLoadEvent()
        {
            sceneLights = FindObjectsOfType<LightController>();
            foreach (LightController light in sceneLights)
            {
                // 改变时间
            }
        }

    }
}