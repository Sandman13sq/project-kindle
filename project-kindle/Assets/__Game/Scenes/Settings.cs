using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{

    public void SetBGMVolume(float volume)
    {
        FindObjectOfType<AudioManager>().SetBGMVolume(volume);
        Sound song = FindObjectOfType<AudioManager>().GetSound("mus_title");
        Debug.Log("Song Volume is: " + song.source.volume);
    }

    public void SetSFXVolume(float volume)
    {
        FindObjectOfType<AudioManager>().SetSFXVolume(volume);
    }
}
