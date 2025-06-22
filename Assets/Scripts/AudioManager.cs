using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{   
    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            audioMixer.SetFloat("music", Mathf.Log10(PlayerPrefs.GetFloat("musicVolume"))*20);
        }

        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            audioMixer.SetFloat("sfx", Mathf.Log10(PlayerPrefs.GetFloat("sfxVolume")) * 20);
        }
    }


    public void playAudio(AudioSource audio)
    {
        audio.Play();
    }

    public void playScheduled(AudioSource audio, double time)
    {
        audio.PlayScheduled(time);
    }

    public void stopAudio(AudioSource audio)
    {
        audio.Stop();
    }

    public void MuteAllAudio(bool isMuted)
    {
        audioMixer.SetFloat("MasterVolume", isMuted ? -80f : 0f);
    }
}
