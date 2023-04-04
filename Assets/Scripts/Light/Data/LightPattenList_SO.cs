using System.Collections.Generic;
using UnityEngine;
using Utility;
namespace Light.Data
{
    [CreateAssetMenu(fileName = "LightPattenList", menuName = "Light/LightPattenList_SO")]
    public class LightPattenList_SO : ScriptableObject
    {
        public List<LightDetails> lightPattenList; 

        public LightDetails GetLightDetail(Season season, LightShift lightShift)
        {
            return lightPattenList.Find((details => details.LightShift == lightShift && details.season == season));
        }
    }
    [System.Serializable]
    public class LightDetails
    {
        public Season season;
        public LightShift LightShift;
        public Color lightColor;
        public float lightAmount;
    }
}