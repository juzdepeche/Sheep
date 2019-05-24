﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource AudioSource;

    public static AudioManager Instance;

    public AudioClip StressMusic;
    public AudioClip MainThemeMusic;

    public Sound[] Sounds;

    private void Awake()
    {
        Instance = this;
        AudioSource = GetComponent<AudioSource>();

        foreach(Sound s in Sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;

            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchToStressMusic()
    {
        ChangeMusic(StressMusic);
    }

    public void SwitchToMainThemeMusic()
    {
        ChangeMusic(MainThemeMusic);
    }

    private void ChangeMusic(AudioClip clip)
    {
        AudioSource.Stop();
        AudioSource.clip = clip;
        AudioSource.Play();
    }

    private void Play(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found.");
            return;
        }
        s.Source.Play();
    }

    public void Bark()
    {
        int animationNumber = UnityEngine.Random.Range(1, 4);
        Play("bark" + animationNumber);
    }

    public void Howl()
    {
        int animationNumber = UnityEngine.Random.Range(1, 3);
        Play("howl" + animationNumber);
    }

    public void Kill()
    {
        int animationNumber = UnityEngine.Random.Range(1, 4);
        Play("kill" + animationNumber);
    }
}
