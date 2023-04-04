using System.Collections;
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

        /// <summary>
        /// 音乐随机多久后播放
        /// </summary>
        private float musicStartSecond => Random.Range(3f, 5f);

        /// <summary>
        /// 音乐的协程
        /// </summary>
        private Coroutine soundRoutine;

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

            if (soundRoutine != null)
            {
                StopCoroutine(soundRoutine);
            }
            else
            {
                soundRoutine = StartCoroutine(PlaySoundRoutine(music, ambient));
            }
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

        private IEnumerator PlaySoundRoutine(SoundDetails music, SoundDetails ambient)
        {
            if (music != null && ambient != null)
            {
                PlayAmbientlip(ambient);
                yield return new WaitForSeconds(musicStartSecond);
                PlaySoundClip(music);
            }
        }

    }
}