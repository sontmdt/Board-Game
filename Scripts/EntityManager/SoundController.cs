using System;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource effectSource;
    public AudioSource musicSource;

    [Header("Audio Clips")]
    public AudioClip switchClip;
    public AudioClip collapseClip;
    public AudioClip openingClip;
    public AudioClip inGameClip;
    //public AudioClip explodeClip;

    private void Awake()
    {
        effectSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        effectSource.volume = 0.5f;
        musicSource.volume = 0.5f;
    }
    public void PlaySwitchSound()
    {
        PlayGameEffect(switchClip);
    }

    public void PlayCollapseSound()
    {
        PlayGameEffect(collapseClip);
    }
    public void PlayOpeningMusic()
    {
        PlayGameMusic(openingClip, true);
    }
    public void PlayInGameMusic()
    {
        PlayGameMusic(inGameClip, true);
    }
    //
    private void PlayGameEffect(AudioClip clip)
    {
        if (clip != null) effectSource.PlayOneShot(clip);
    }

    private void PlayGameMusic(AudioClip clip, bool loop = true)
    {
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopGameMusic()
    {
        musicSource.Stop();
    }
    public void OnVolumeMusicChanged(float musicVolume)
    {
        musicSource.volume = musicVolume;
    }
    public void OnVolumeSoundChanged(float effectVolume)
    {
        effectSource.volume = effectVolume;
    }
}
