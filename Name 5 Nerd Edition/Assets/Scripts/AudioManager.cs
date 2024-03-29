using System;
using UnityEngine.Audio;
using UnityEngine;

//from brackey's tutorial


public class AudioManager : MonoBehaviour
{
    public Sound[] soundsArray;

    private void Awake()
    {
        foreach (Sound s in soundsArray)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void PlaySound(string soundName)
    {
       Sound s = Array.Find(soundsArray, sound => sound.name == name);
       if (s == null)
       {
           return;
       }
       s.source.Play();
    }
}
