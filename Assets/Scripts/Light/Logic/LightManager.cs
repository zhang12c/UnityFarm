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

        private float _timeDifference;

        private void OnEnable()
        {
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            MyEventHandler.LightShiftChangeEvent += OnLightShiftChangeEvent;
        }
        private void OnDisable()
        { 
            MyEventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            MyEventHandler.LightShiftChangeEvent -= OnLightShiftChangeEvent;
        }
        private void OnLightShiftChangeEvent(Season s, LightShift l, float t)
        {
            _season = s;
            _timeDifference = t;
            if (currentLightShift != l)
            {
                // 需要切换灯
                currentLightShift = l;

                if (sceneLights.Length > 0)
                    foreach (LightController light in sceneLights)
                    {
                        // 改变
                        light.ChangeLight(s,l,t);
                    }
            }
            
        }
        private void OnAfterSceneLoadEvent()
        {
            sceneLights = FindObjectsOfType<LightController>();
            foreach (LightController light in sceneLights)
            {
                // 改变时间
                light.ChangeLight(_season,currentLightShift,_timeDifference);
            }
        }

    }
}