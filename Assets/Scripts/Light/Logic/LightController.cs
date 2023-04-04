using System;
using Light.Data;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
    }
}