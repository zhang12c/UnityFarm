using System;
using Audio.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;
namespace Audio.Logic
{
    public class AudioManager : MonoBehaviour
    {
        public SoundDetailsList_SO soundDetailsData;
        public SceneSoundList_SO sceneSoundData;

        public AudioSource gameBgAudioSource;
        public AudioSource ambientAudioSource;

        private void OnEnable()
        {
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        }

        private void OnDisable()
        {
            MyEventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;

        }
        private void OnAfterSceneLoadEvent()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneSoundItem sceneSound = sceneSoundData.GetSceneSoundItem(currentScene);

            if (sceneSound == null)
                return;

            SoundDetails ambient = soundDetailsData.GetSoundDetail(sceneSound.ambient);
            SoundDetails music = soundDetailsData.GetSoundDetail(sceneSound.Music);
            
            PlaySoundClip(music);
            PlayAmbientlip(ambient);
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="soundDetails"></param>
        private void PlaySoundClip(SoundDetails soundDetails)
        {
            gameBgAudioSource.clip = soundDetails.soundClip;
            if (gameBgAudioSource.isActiveAndEnabled)
            {
                gameBgAudioSource.Play();
            }
        }
        
        /// <summary>
        /// 播放环境音乐
        /// </summary>
        /// <param name="soundDetails"></param>
        private void PlayAmbientlip(SoundDetails soundDetails)
        {
            ambientAudioSource.clip = soundDetails.soundClip;
            if (ambientAudioSource.isActiveAndEnabled)
            {
                ambientAudioSource.Play();
            }
        }

    }
}