using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SP_SettingsPersistence : MonoBehaviour
{
    public static SP_SettingsPersistence instance;

    public bool isVoiceLinesEnabled = true;
    public bool isSoundEnabled = true;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadSavedSettings();
    }

    private void LoadSavedSettings()
    {
        if (SP_LevelAudioControl.instance != null)
        {
            if (SP_LevelAudioControl.instance.isVoiceAssistOn != isVoiceLinesEnabled)
                SP_LevelAudioControl.instance.ToggleVoiceLines();
            if (SP_LevelAudioControl.instance.isSoundOn != isSoundEnabled)
                SP_LevelAudioControl.instance.ToggleSound();
        }
    }
    public void SaveSoundSettings(bool isSoundOn)
    {
        isSoundEnabled = isSoundOn;
    }

    public void SaveVoiceSettings(bool isVoiceOn)
    {
        isVoiceLinesEnabled = isVoiceOn;
    }
}
