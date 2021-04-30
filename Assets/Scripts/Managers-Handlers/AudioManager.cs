using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource MusicAudioSource;
    public AudioSource SoundsAudioSource;
    public Audio MainMusic;
    public Audio CannonSound;
    public Audio PopSound;
    public Audio StageWinSound;
    public Audio LevelWinSound;



    [System.Serializable]
    public class Audio
    {
        public AudioClip Clip;
        public float Volume;
    }

    private void Start()
    {
        PlayMainMusic();
    }

    public void PlayMainMusic()
    {
        MusicAudioSource.clip = MainMusic.Clip;
        MusicAudioSource.volume = MainMusic.Volume;
        MusicAudioSource.Play();
    }

    public void PlayCannonSound()
    {
        SoundsAudioSource.PlayOneShot(CannonSound.Clip, CannonSound.Volume);
    }

    public void PlayPopSound()
    {
        SoundsAudioSource.PlayOneShot(PopSound.Clip, PopSound.Volume);
    }

    public void PlayStageWinSound()
    {
        SoundsAudioSource.PlayOneShot(StageWinSound.Clip, StageWinSound.Volume);
    }

    public void PlayLevelWinSound()
    {
        SoundsAudioSource.PlayOneShot(LevelWinSound.Clip, LevelWinSound.Volume);
    }

    public void OrderToSetMainMusicVolume(float volume, float secondsToResetVolume)
    {
        StartCoroutine(SetMainMusicVolume(volume, secondsToResetVolume));
    }

    public IEnumerator SetMainMusicVolume(float volume, float secondsToResetVolume)
    {
        float oldVoume = MusicAudioSource.volume;
        MusicAudioSource.volume = volume;
        yield return new WaitForSeconds(secondsToResetVolume);
        MusicAudioSource.volume = oldVoume;
    }

    public void UpdateAudioMuteState(bool hasToMute)
    {
        if (hasToMute)
        {
            MusicAudioSource.volume = 0;
            SoundsAudioSource.volume = 0;
        }
        else
        {
            MusicAudioSource.volume = 1;
            SoundsAudioSource.volume = 1;
        }
    }
}
