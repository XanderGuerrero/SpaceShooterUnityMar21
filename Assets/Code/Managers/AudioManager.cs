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
    private AudioSource sfxSource2;
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
        sfxSource2 = this.gameObject.AddComponent<AudioSource>();
        //loop the music tracks
        musicSource.loop = false;
        //musicSource2.loop = true;
        sfxSource.loop = false;
        sfxSource2.loop = true;
        sfxSource2.pitch = .7f;
    }

    public void Start()
    {
        //SetMusicVolume(1.0f);
        PlayMusicWithFade(BGM[trackCount], musicSource);
        //PlayMusic(BGM[11], musicSource2);
        //trackCount++;
    }

    public void Update()
    {
        //determine which source is active
        //AudioSource activeSource = (firstMusicSourceIsPlaying) ? musicSource : musicSource2;
        AudioSource BGMSource = musicSource;
        if (BGMSource.clip != null)
        {
            //time = Time.time;
            //Debug.Log(time);
            if (!BGMSource.isPlaying)
            {
                //time = 0.0f;
                trackCount++;
                Debug.Log(trackCount);
                PlayMusicWithFade(BGM[trackCount], musicSource);
                
                if (trackCount == 10)
                    trackCount = 0;
            }
        }
     
    }


    public void PlayMusic(AudioClip musicClip, AudioSource source)
    {
        //determine which source is active
        //AudioSource activeSource = (firstMusicSourceIsPlaying) ? musicSource : musicSource2;
        Debug.Log(musicClip.name);
        Debug.Log(BGM[11].name);
        if (musicClip.name == BGM[11].name)
        {
            source.clip = musicClip;
            source.volume = .7f;
            source.pitch = .7f;
            source.loop = true;
            source.Play();
            Debug.Log("Audio clip length : " + source.clip.length);
        }
        else
        {
            source.clip = musicClip;
            source.volume = .75f;
            source.Play();
            Debug.Log("Audio clip length : " + source.clip.length);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlaySFX2(string clip, float volume)
    {
        Debug.Log(clip);
      
        //s = Array.Find(sounds, sound => sound.name == name);
        sfxSource2.clip = BGM.Find(x => x.name == clip);
        Debug.Log(sfxSource2.clip.name);
        sfxSource2.volume = volume;
        sfxSource2.Play();
        
    }

    public void StopSFX2(string clip)
    {
        sfxSource2.Stop();
    }

    public void StopSFX(string clip)
    {

        sfxSource.Stop();
    }

    public void PlayMusicWithFade(AudioClip newClip, AudioSource source, float transitionTime = 1.0f)
    {
        //determine which source is active
        //AudioSource activeSource = (firstMusicSourceIsPlaying) ? musicSource : musicSource2;
        source.volume = .75f;
        StartCoroutine(UpdateMusicWithFade(source, newClip, transitionTime, source.volume));

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
        StartCoroutine(UpdateMusicWithCrossFade(activeSource, newSource, transitionTime, activeSource.volume));

    }

    private IEnumerator UpdateMusicWithCrossFade(AudioSource original, AudioSource newSource, float transitionTime, float musicVolume)
    {
        float t = 0.0f;
        for (t = 0.0f; t < transitionTime; t += Time.deltaTime)
        {
            original.volume = (musicVolume - ((t / transitionTime)* musicVolume));
            newSource.volume = (t / transitionTime) * musicVolume;
            yield return null;
        }

        original.Stop();
    }

    private IEnumerator UpdateMusicWithFade(AudioSource activeSource, AudioClip newClip, float transitionTime, float musicVolume)
    {
        //make sure the source is avtive and playing
        if (!activeSource.isPlaying)
        {
            activeSource.Play();

            float t = 0.0f;

            //Fade Out
            for (t = 0; t <= transitionTime; t += Time.deltaTime)
            {
                activeSource.volume = (musicVolume - ((t / transitionTime)* musicVolume));
                yield return null;
            }

            activeSource.Stop();
            activeSource.clip = newClip;
            activeSource.Play();

            //Fade In
            for (t = 0; t <= transitionTime; t +=Time.deltaTime)
            {
                activeSource.volume = (t / transitionTime) * musicVolume;
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
