using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{

    public AudioSource efxSource;

    public AudioSource musicSource;

    public static SoundManager instance;

    public float lowPitchRange = .95f;

    public float highPitchRange = 1.05f;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySingle(AudioClip audio)
    {
        efxSource.clip = audio;
        efxSource.Play();
    }

    public void RandomizeSFX(params AudioClip[] audioClips)
    {
        int randomIndex = Random.Range(0, audioClips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        efxSource.pitch = randomPitch;
        efxSource.clip = audioClips[randomIndex];
        efxSource.Play();
    }
    
}
