using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioMixerGroup mixerGroup;

    public bool loop;

    [Range(0f, 1f)] public float volume;

    public bool followingSounds;
    public bool randomSounds;

    public int currentIndex;
    public int rndIndex;

    public int clipsNumber;
    [HideInInspector] public AudioClip clip;
    [HideInInspector] public AudioClip[] multipleClips;

    [HideInInspector] public AudioSource source;
}

