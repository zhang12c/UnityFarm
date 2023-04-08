using System;
using UnityEngine;
using Utility;
namespace Light.Logic
{
    public class LightManager : MonoBehaviour
    {
        private LightController[] _sceneLights;

        private LightShift _currentLightShift;

        private Season _season;

        private float _timeDifference;

        private void OnEnable()
        {
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            MyEventHandler.LightShiftChangeEvent += OnLightShiftChangeEvent;
            MyEventHandler.StartNewGameEvent += OnStartNewGameEvent;

        }
        private void OnDisable()
        { 
            MyEventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            MyEventHandler.LightShiftChangeEvent -= OnLightShiftChangeEvent;
            MyEventHandler.StartNewGameEvent -= OnStartNewGameEvent;

        }
        private void OnStartNewGameEvent(int obj)
        {
            _currentLightShift = LightShift.Morning;
        }
        private void OnLightShiftChangeEvent(Season s, LightShift l, float t)
        {
            _season = s;
            _timeDifference = t;
            if (_currentLightShift != l)
            {
                // 需要切换灯
                _currentLightShift = l;

                if (_sceneLights.Length > 0)
                    foreach (LightController controller in _sceneLights)
                    {
                        // 改变
                        controller.ChangeLight(s,l,t);
                    }
            }
            
        }
        private void OnAfterSceneLoadEvent()
        {
            _sceneLights = FindObjectsOfType<LightController>();
            foreach (LightController controller in _sceneLights)
            {
                // 改变时间
                controller.ChangeLight(_season,_currentLightShift,_timeDifference);
            }
        }

    }
}