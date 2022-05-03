using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;
    // initialize stuff
    void Awake()
    {
        // Enforce singleton behavior
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public Sound Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Cannot find the sound \""+ name +"\". Did you misspell something?");
            return s;
        }
        s.source.Play();

        return s;
    }   

    public Sound Stop (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Cannot find the sound \""+ name +"\". Did you misspell something?");
            return s;
        }
        s.source.Stop();

        return s;
    }   

    public Sound GetSound(string name)
    {
        return Array.Find(sounds, sound => sound.name == name);
    }

    public void SetSFXVolume(float volume)
    {
        foreach (Sound s in sounds)
        {
            //1 = SFX
            if(s.type == 1)
                s.volume = volume;
                s.source.volume = s.volume;
        }
    }

    public void SetBGMVolume(float volume)
    {
        foreach (Sound s in sounds)
        {
            //0 = BGM
            if(s.type == 0)
            {
                s.volume = volume;
                s.source.volume = s.volume;
            }
        }
    }
}
