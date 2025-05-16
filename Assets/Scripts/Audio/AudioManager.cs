using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : GameMonoBehaviour
{

    public static AudioManager Instance;
    public Sound[] musicSounds;
    public Sound[] sfxSounds;

    public AudioSource musicSource;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        foreach (var item in sfxSounds)
        {
            item.source = gameObject.AddComponent<AudioSource>();
            item.source.clip = item.clip;
            item.source.volume = item.volume;
            item.source.pitch = item.pitch;
        }
        foreach (var item in musicSounds)
        {
            item.source = gameObject.AddComponent<AudioSource>();
            item.source.clip = item.clip;
            item.source.volume = item.volume;
            item.source.pitch = item.pitch;
        }
    }

    protected override void Start()
    {
        base.Start();
        PlayMusic("MenuMusic");
    }

    public void PlayMusic(string name)
    {
        Sound sound = System.Array.Find(musicSounds, s => s.name == name);
        if (sound == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        musicSource.clip = sound.clip;
        musicSource.Play();
    }
    public void PlaySFX(string name)
    {
        Sound sound = System.Array.Find(sfxSounds, s => s.name == name);
        if (sound == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        sound.source.PlayOneShot(sound.clip);
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        foreach (var item in sfxSounds)
        {
            item.source.mute = !item.source.mute;
        }
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        foreach (var item in sfxSounds)
        {
            item.source.volume = item.source.volume * volume;
        }
    }
}
