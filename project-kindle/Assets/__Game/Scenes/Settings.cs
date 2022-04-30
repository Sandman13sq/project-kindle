using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private _GameHeader header;
    public void SetBGMVolume(float volume)
    {
        FindObjectOfType<AudioManager>().SetBGMVolume(volume);
        header.audiosource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        FindObjectOfType<AudioManager>().SetSFXVolume(volume);
    }
}
