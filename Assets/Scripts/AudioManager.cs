using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEditor;

public class AudioManager : MonoBehaviour
{
    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;

    [Header("Sound List")]
    public Sound[] sounds;

    public static AudioManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // for debug
        //for (int i = 0; i < sounds.Length; i++)
        //{
        //    sounds[i].source = gameObject.AddComponent<AudioSource>();
        //    sounds[i].clip = Resources.Load<AudioClip>("SFX Debug/" + (i + 1));
        //    sounds[i].source.clip = sounds[i].clip;
        //    sounds[i].source.volume = sounds[i].volume;
        //    //sounds[i].source.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        //    sounds[i].source.loop = sounds[i].loop;
        //}

        //for using actual sfx with correct names
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.clip = Resources.Load<AudioClip>("Audio/" + s.name);
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            /*s.source.pitch = unityengine.random.range(0.95f,1.05f);*/
            s.source.loop = s.loop;
        }

        //Play("music");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        s.source.Play();
    }
}
