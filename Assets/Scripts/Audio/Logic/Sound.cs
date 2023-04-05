using Audio.Data;
using UnityEngine;
namespace Audio.Logic
{
    [RequireComponent(typeof(AudioSource))]
    public class Sound : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        public void SetSound(SoundDetails soundDetails)
        {
            _audioSource.clip = soundDetails.soundClip;
            _audioSource.volume = soundDetails.soundVolume;
            _audioSource.pitch = Random.Range(soundDetails.soundPitchMin, soundDetails.soundPitchMax);
        }
    }
}