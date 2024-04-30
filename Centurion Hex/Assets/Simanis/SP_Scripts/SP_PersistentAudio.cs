using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_PersistentAudio : MonoBehaviour
{
    public static SP_PersistentAudio instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
