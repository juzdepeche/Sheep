using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource AudioSource;

    public static AudioManager Instance;

    public AudioClip StressMusic;
    public AudioClip MainThemeMusic;

    private void Awake()
    {
        Instance = this;
        AudioSource = GetComponent<AudioSource>();
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
}
