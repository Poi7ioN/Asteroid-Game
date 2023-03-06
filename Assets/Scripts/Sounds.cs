using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


//This is a serizlized class for sounds 
[System.Serializable]
public class Sounds
{
    public string name;                                      //name of audio clip

    public AudioClip clip;                                 //audio clip file

    [Range(0f,1f)]   public float volume = 1f;           // volume of clip
    [Range(0.1f,3f)] public float pitch  = 1f;          // pitch of clip
    public bool loop;                                  // check to loop or not
    public bool playOnAwake;                          //check to play on awake
    
    [HideInInspector] public AudioSource source;

}
