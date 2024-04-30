using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_PersistentAudio : MonoBehaviour
{
    public static SP_PersistentAudio instance;
    private AudioSource audioSource;
    public bool isMusicOn = true;

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
        DontDestroyOnLoad(this.gameObject);

        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void ToggleMusic()
    {
        
        if (isMusicOn)
        {
            audioSource.Stop();
        }
        else
        {
            audioSource.Play();
        }
        isMusicOn = !isMusicOn;
    }
}
