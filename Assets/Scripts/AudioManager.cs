using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;

    public Sound[] sounds;

    private float timeToPlay = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.playOnAwake = s.playOnAwake;
            s.source.loop = s.loop;
            s.source.mute = s.mute;
        }
    }

    public void Play(string name)
    {
        if(name == "walk" || name == "water")
        {
            if(canPlay(name))
            {
                Sound s = Array.Find(sounds, s => s.name == name);

                if(s != null) s.source.PlayOneShot(s.clip);
            }
        }
        else
        {
            Sound s = Array.Find(sounds, s => s.name == name);

            if(s != null) s.source.PlayOneShot(s.clip);
        }
        
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, s => s.name == name);

        if(s != null) s.source.Stop();
    }

    private bool canPlay(string name)
    {
        bool answer = false;

        Sound s = Array.Find(sounds, s => s.name == name);

        if(s.name == "walk")
        {
            float nexTimeToPlay = 1.032f;
            if(timeToPlay + nexTimeToPlay < Time.time)
            {
                timeToPlay = Time.time;
                answer = true;
            }
        }
        else if(s.name == "water")
        {
            float nexTimeToPlay = 1f;
            if(timeToPlay + nexTimeToPlay < Time.time)
            {
                timeToPlay = Time.time;
                answer = true;
            }
        }
        else if(s.name == "wind")
        {
            float nexTimeToPlay = 7f;
            if(timeToPlay + nexTimeToPlay < Time.time)
            {
                timeToPlay = Time.time;
                answer = true;
            }
        }

        return answer;
    }
}
