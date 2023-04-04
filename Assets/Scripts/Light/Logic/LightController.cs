using System;
using DG.Tweening;
using Light.Data;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utility;
namespace Light.Logic
{
    public class LightController : MonoBehaviour
    {
        public LightPattenList_SO lightData;

        private Light2D _light2D;
        private LightDetails _lightDetails;

        private void Awake()
        {
            _light2D = GetComponent<Light2D>();
        }
        /// <summary>
        /// 切换一下灯
        /// </summary>
        /// <param name="s">季节</param>
        /// <param name="l">早上还是晚上</param>
        /// <param name="t">时间差</param>
        public void ChangeLight(Season s, LightShift l, float t)
        {
            _lightDetails = lightData.GetLightDetail(s, l);

            if (Settings.lightChangeDuration > t)
            {
                Color colorOffset = (_lightDetails.lightColor - _light2D.color) / Settings.lightChangeDuration * t;
                _light2D.color += colorOffset;
                DOTween.To(() => _light2D.color, c => _light2D.color = c, _lightDetails.lightColor, Settings.lightChangeDuration - t);
                DOTween.To(() => _light2D.intensity, i => _light2D.intensity = i, _lightDetails.lightAmount, Settings.lightChangeDuration - t);
            }
            if (t >= Settings.lightChangeDuration)
            {
                _light2D.color = _lightDetails.lightColor;
                _light2D.intensity = _lightDetails.lightAmount;
            }
        }
    }
}