using System;
using UnityEngine;

[System.Serializable()]
public struct SoundParameters
{
    [Range(0, 1)]
    public float Volume;
    [Range(-3, 3)]
    public float Pitch;
    public bool Loop;
}
[System.Serializable()]
public class Sound
{
    #region Variables

    [SerializeField] String name = String.Empty;
    public String Name { get { return name; } }

    [SerializeField] AudioClip clip = null;
    public AudioClip Clip { get { return clip; } }

    [SerializeField] SoundParameters parameters = new SoundParameters();
    public SoundParameters Parameters { get { return parameters; } }

    [HideInInspector]
    public AudioSource Source = null;

    #endregion

    public void Play()
    {
        Source.clip = Clip;
        Source.pitch = Parameters.Pitch;
        Source.loop = Parameters.Loop;
        Source.volume = Parameters.Volume;
        Source.Play();
    }
    public void StopGM()
    {
        Source.Stop();
    }
}
public class AudioManager : MonoBehaviour
{
    #region Variables

    public static AudioManager Instance = null;

    [SerializeField] Sound[] sounds = null;
    [SerializeField] AudioSource sourcePrefab = null;
    [SerializeField] String startupTrack = String.Empty;
    public String startupTrackk { get { return startupTrack; } }
    bool sfxVolume = false;

    #endregion

    #region Default Unity methods

    private void Awake()
    {
        if (Instance != null)
        { Destroy(gameObject); }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        InitSounds();
    }

    #endregion

    private void InitSounds()
    {
        foreach (var sound in sounds)
        {
            AudioSource source = (AudioSource)Instantiate(sourcePrefab, gameObject.transform);
            source.name = sound.Name;
            sound.Source = source;
        }
    }

    public void PlaySoundFX(string name)
    {
        var sound = GetSound(name);
        if (sound != null)
        {
            if (sfxVolume) sound.Play();
        }
        else
            Debug.LogWarning("Sound by the name " + name + " is not found! Issues occured at AudioManager.StopSound()");
    }

    public void PlayGameMusic()
    {
        sounds[0].Play();
    }

    public void StopGameMusic()
    {
        sounds[0].StopGM();
    }

    public void ChangeSFXVolume(bool isOn)
    {
        sfxVolume = isOn;
    }

    public void DestroyAudioMng()
    {
        Destroy(gameObject);
    }

    #region Getters

    Sound GetSound(string name)
    {
        foreach (var sound in sounds)
        {
            if (sound.Name == name)
            {
                return sound;
            }
        }
        return null;
    }

    #endregion
}