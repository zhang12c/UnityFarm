using System.Collections.Generic;
using UnityEngine;
using Utility;
namespace Audio.Data
{
    [CreateAssetMenu(fileName = "SoundDetailsList", menuName = "Audio/SoundDetailList_SO")]
    public class SoundDetailsList_SO : ScriptableObject
    {
        public List<SoundDetails> _SoundDetailsList;

        public SoundDetails GetSoundDetail(SoundName name)
        {
            return _SoundDetailsList.Find((details => details.soundName == name));
        }
    }

    [System.Serializable]
    public class SoundDetails
    {
        public SoundName soundName;
        public AudioClip soundClip;
        /// <summary>
        /// 音调
        /// </summary>
        [Range(0,2)]
        public float soundPitchMin = 0.8f;
        [Range(0,2)]
        public float soundPitchMax = 1.2f;
        /// <summary>
        /// 音量大小
        /// </summary>
        [Range(0,2)]
        public float soundVolume = 0.2f;
    }
}