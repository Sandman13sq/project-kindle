using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MasterObject
{
    public void SetBGMVolume(float volume)
    {
        FindObjectOfType<AudioManager>().SetBGMVolume(volume);
        game.SetBGMVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        FindObjectOfType<AudioManager>().SetSFXVolume(volume);
    }
}
