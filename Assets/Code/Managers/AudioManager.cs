using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    #region Static Instance
    private static AudioManager instance;
    public List<AudioClip> BGM;
    public static AudioManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
                if(instance == null)
                {
                    instance = new GameObject("Spawned Audio Manager", typeof(AudioManager)).GetComponent<AudioManager>();
                }
            }
            return instance;
        }

        private set
        {
            instance = value;
        }
    }
    #endregion

    #region Fields
    private AudioSource musicSource;
    private AudioSource musicSource2;
    private AudioSource sfxSource;
    private bool firstMusicSourceIsPlaying;
    int trackCount = 0;
    float time = 0.0f;
    #endregion

    private void Awake()
    {
        //make sure we dont destroy this instance
        DontDestroyOnLoad(this.gameObject);

        musicSource = this.gameObject.AddComponent<AudioSource>();
        musicSource2 = this.gameObject.AddComponent<AudioSource>();
        sfxSource = this.gameObject.AddComponent<AudioSource>();

        //loop the music tracks
        musicSource.loop = false;
        musicSource2.loop = false;
        sfxSource.loop = false;
    }

    public void Start()
    {
        SetMusicVolume(1.0f);
        PlayMusicWithFade(BGM[trackCount]);
        //trackCount++;
    }

    public void Update()
    {
        //determine which source is active
        AudioSource activeSource = (firstMusicSourceIsPlaying) ? musicSource : musicSource2;
        if(activeSource.clip != null)
        {
            //time = Time.time;
            //Debug.Log(time);
            if (!activeSource.isPlaying)
            {
                //time = 0.0f;
                trackCount++;
                Debug.Log(trackCount);
                PlayMusicWithFade(BGM[trackCount], 1.0f);
                
                if (trackCount == 10)
                    trackCount = 0;
            }
        }
     
    }


    public void PlayMusic(AudioClip musicClip)
    {
        //determine which source is active
        AudioSource activeSource = (firstMusicSourceIsPlaying) ? musicSource : musicSource2;

        activeSource.clip = musicClip;
        activeSource.volume = 1;
        activeSource.Play();
        Debug.Log("Audio clip length : " + activeSource.clip.length);
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayMusicWithFade(AudioClip newClip, float transitionTime = 1.0f)
    {
        //determine which source is active
        AudioSource activeSource = (firstMusicSourceIsPlaying) ? musicSource : musicSource2;
        StartCoroutine(UpdateMusicWithFade(activeSource, newClip, transitionTime));

    }

    public void PlayMusicWithCrossFade(AudioClip musicClip, float transitionTime = 1.0f)
    {
        //determine which source is active
        AudioSource activeSource = (firstMusicSourceIsPlaying) ? musicSource : musicSource2;
        AudioSource newSource = (firstMusicSourceIsPlaying) ? musicSource2 : musicSource;

        //swap the source
        firstMusicSourceIsPlaying = !firstMusicSourceIsPlaying;

        //Set the fields of the aduio source, then start the coroutine to crosfade
        newSource.clip = musicClip;
        newSource.Play();
        StartCoroutine(UpdateMusicWithCrossFade(activeSource, newSource, transitionTime));

    }

    private IEnumerator UpdateMusicWithCrossFade(AudioSource original, AudioSource newSource, float transitionTime)
    {
        float t = 0.0f;
        for (t = 0.0f; t <= transitionTime; t += Time.deltaTime)
        {
            original.volume = (1 - (t / transitionTime));
            newSource.volume = (t / transitionTime);
            yield return null;
        }

        original.Stop();
    }

    private IEnumerator UpdateMusicWithFade(AudioSource activeSource, AudioClip newClip, float transitionTime)
    {
        //make sure the source is avtive and playing
        if (!activeSource.isPlaying)
        {
            activeSource.Play();

            float t = 0.0f;

            //Fade Out
            for (t = 0; t < transitionTime; t += Time.deltaTime)
            {
                activeSource.volume = (1 - (t / transitionTime));
                yield return null;
            }

            activeSource.Stop();
            activeSource.clip = newClip;
            activeSource.Play();

            //Fade In
            for (t = 0; t < transitionTime; t +=Time.deltaTime)
            {
                activeSource.volume = (t / transitionTime);
                yield return null;
            }
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        musicSource2.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        musicSource.volume = volume;
    }

    //public Sound[] sounds;
    //Sound s;
    //public static AudioManager instance;

    //private void Awake()
    //{
    //    if(instance == null)
    //    {
    //        instance = this;
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }

    //    DontDestroyOnLoad(gameObject);

    //    foreach(Sound s in sounds)
    //    {
    //        s.source = gameObject.AddComponent<AudioSource>();
    //        s.source.clip = s.clip;

    //        s.source.volume = s.volume;
    //        s.source.pitch = s.pitch;
    //        s.source.loop = s.loop;
    //    }
    //}

    //private void Start()
    //{
    //    Play("Theme");
    //}

    //private void Update()
    //{
    //    //Play("Theme");

    //    if (s.source.name == "Theme" && s.source.isPlaying == false)
    //    {
    //        Play("Theme2");
    //    }
    //    else if(s.source.name == "Theme2" && s.source.isPlaying == false)
    //    {
    //        Play("Theme3");
    //    }
    //    else if (s.source.name == "Theme3" && s.source.isPlaying == false)
    //    {
    //        Play("Theme4");
    //    }
    //    else if (s.source.name == "Theme4" && s.source.isPlaying == false)
    //    {
    //        Play("Theme5");
    //    }
    //    else if (s.source.name == "Theme5" && s.source.isPlaying == false)
    //    {
    //        Play("Theme");
    //    }
    //}

    //public void Play(string name)
    //{
    //     s = Array.Find(sounds, sound => sound.name == name);
    //    if(s == null)
    //    {
    //        Debug.LogWarning("Sound: " + name + "not found!");
    //        return; 
    //    }

    //    s.source.Play();
    //}
}
