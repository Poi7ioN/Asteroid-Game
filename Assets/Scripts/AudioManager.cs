using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    [SerializeField] int noOfSources = 6;          // number of audio sources to create

    public static AudioManager _instance;          // static reference to AudioManager instance

    public static AudioManager Instance { get { return _instance; } } // getter for AudioManager instance

    public Sounds[] sounds;                       // array of sounds objects that holds the clip details.

    public List<AudioSource> ClipSource;         // list of audio sources.

    private void Awake()
    {
        // singleton implementation
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(gameObject);                     // don't destroy AudioManager on scene change


        for (var i = 0; i < noOfSources; i++)            //   // create audio sources and add them to the ClipSource list
        {
            ClipSource.Add(gameObject.AddComponent<AudioSource>());
        }
    }

    //Button Clip Functions.
    /// <summary>
    /// 0-Single Fire, 1- Spread, 2- Asteroid, 3-Ship, 4-Shield
    /// </summary>
    /// <param name="index"></param>
    /// <param name="clipName"></param>
    public void PlayAudioClip(int index, string clipName)       // play audio clip with index and clipname mentioned in inspector.
    {
        Sounds s = Array.Find(sounds, sound => sound.name == clipName);  // find the sound with the given name
        if (s == null)
            return;

        SetClipProperties(index, s);
        ClipSource[index].Play();
    }

    private void SetClipProperties(int index, Sounds s)   //set all the clip properties from sounds class
    {
        ClipSource[index].clip = s.clip;
        ClipSource[index].volume = s.volume;
        ClipSource[index].pitch = s.pitch;
        ClipSource[index].loop = s.loop;
        ClipSource[index].playOnAwake = s.playOnAwake;
    }

    public void StopAudioClip(int index)  // stop a audio clip
    {
        ClipSource[index].Stop();
    }
}
