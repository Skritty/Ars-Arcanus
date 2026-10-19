// Written by: Trevor Thacker
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains functions to call the audio manager from designer events. More options can be had by using the audio manager directly in a script.
/// </summary>
public class AudioController : MonoBehaviour
{
    public void PlayMusic(AudioTrack track)
    {
        AudioManager.Instance.PlayMusic(track);
    }

    public void PlayAmbience(AudioTrack track)
    {
        AudioManager.Instance.PlayAmbience(track);
    }

    public void PlaySoundNoPosition(AudioClip clip)
    {
        AudioManager.Instance.PlaySound(clip, Camera.main.transform.position);
    }

    public void PlaySFX(SoundEffect sfx)
    {
        AudioManager.Instance.PlaySFX(sfx, Camera.main.transform.position);
    }

    public void StopAllSound()
    {
        AudioManager.Instance.StopAllSounds();
    }
}
