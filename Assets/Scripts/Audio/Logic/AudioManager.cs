using System.Collections;
using Audio.Data;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Utility;
namespace Audio.Logic
{
    public class AudioManager : Singleton<AudioManager>
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

        [Header("Snapshots")]
        public AudioMixerSnapshot normalSnapShot;
        public AudioMixerSnapshot ambientSnapShot;
        public AudioMixerSnapshot muteSnapShot;
        [Header("Audio Mixer")]
        public AudioMixer audioMixer;

        /// <summary>
        /// 音量快照切换的事件间隔
        /// </summary>
        private float musicTransitionSecond = 8f;

        private void OnEnable()
        {
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            MyEventHandler.PlaySoundEvent += OnPlaySoundEvent;
        }

        private void OnDisable()
        {
            MyEventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            MyEventHandler.PlaySoundEvent -= OnPlaySoundEvent;

        }
        private void OnPlaySoundEvent(SoundName obj)
        {
            var soundDetails = soundDetailsData.GetSoundDetail(obj);
            if (soundDetails != null)
            {
                MyEventHandler.CallInitSoundEffect(soundDetails);
            }
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
        private void PlaySoundClip(SoundDetails soundDetails,float transitionTime)
        {
            audioMixer.SetFloat("MusicVolume",ConvertSoundVolume(soundDetails.soundVolume));
            gameBgAudioSource.clip = soundDetails.soundClip;
            if (gameBgAudioSource.isActiveAndEnabled)
            {
                gameBgAudioSource.Play();
            }
            
            normalSnapShot.TransitionTo(transitionTime);
        }
        
        /// <summary>
        /// 播放环境音乐
        /// </summary>
        /// <param name="soundDetails"></param>
        private void PlayAmbientlip(SoundDetails soundDetails,float transitionTime)
        {
            audioMixer.SetFloat("AmbientVolume",ConvertSoundVolume(soundDetails.soundVolume));
            ambientAudioSource.clip = soundDetails.soundClip;
            if (ambientAudioSource.isActiveAndEnabled)
            {
                ambientAudioSource.Play();
            }
            normalSnapShot.TransitionTo(transitionTime);
        }

        private IEnumerator PlaySoundRoutine(SoundDetails music, SoundDetails ambient)
        {
            if (music != null && ambient != null)
            {
                PlayAmbientlip(ambient,musicTransitionSecond);
                yield return new WaitForSeconds(musicStartSecond);
                PlaySoundClip(music,musicTransitionSecond);
            }
        }

        /// <summary>
        /// 将音量0 - 1.5f 转化 到 比特
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        private float ConvertSoundVolume(float amount)
        {
            return (amount * 100 - 80);
        }

    }
}