using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_LevelAudioControl : MonoBehaviour
{
    public static SP_LevelAudioControl instance;

    public AudioClip insuffcientSpeedSFX;
    public AudioClip outOfRangeSFX;
    public AudioClip sleepingSFX;
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
        audioSource = gameObject.GetComponent<AudioSource>();
    }
    
    public void PlayInsuffcientSpeedSFX()
    {
        audioSource.PlayOneShot(insuffcientSpeedSFX);
    }

    public void PlaySFX(AudioClip sfx)
    {
        audioSource.PlayOneShot(sfx);
    }
}
