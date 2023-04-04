using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;
namespace Audio.Data
{
    [CreateAssetMenu(fileName = "SceneSoundList", menuName = "Audio/SceneSoundList_SO")]
    public class SceneSoundList_SO : ScriptableObject
    {
        public List<SceneSoundItem> sceneSoundList;

        public SceneSoundItem GetSceneSoundItem(string sceneName)
        {
            return sceneSoundList.Find((item => item.sceneName == sceneName));
        }
    }

    [Serializable]
    public class SceneSoundItem
    {
        [SceneName] public string sceneName;
        public SoundName ambient;
        public SoundName Music;
    }
}