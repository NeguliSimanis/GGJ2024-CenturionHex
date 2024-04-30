using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SP_Settings : MonoBehaviour
{
    public static SP_Settings instance;
    [HideInInspector] public bool isSettingsOpen = false;

    public GameObject settingsBG;
    public GameObject settingsContainer;
    public GameObject settingsSaver;

    public Toggle musicToggle;
    public Toggle soundToggle;
    public Toggle voiceLineToggle;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
       
    }

    private void Update()
    {
        if (!isSettingsOpen)
            return;

        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OpenSettings(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Escape))
        {
            LoadMainMenuScene();
        }

    }

    public void ToggleSFX()
    {
        if (SP_LevelAudioControl.instance != null)
        {
            Debug.Log("toggling sfx");
            SP_LevelAudioControl.instance.ToggleSound();
            
        }
        if (SP_SettingsPersistence.instance == null)
            Instantiate(settingsSaver);
        SP_SettingsPersistence.instance.SaveSoundSettings(soundToggle.isOn);
    }

    public void ToggleMusic()
    {
        if (SP_PersistentAudio.instance != null)
        {
            SP_PersistentAudio.instance.ToggleMusic();
        }

    }

    public void TogggleVoiceLines()
    {
        if (SP_LevelAudioControl.instance != null)
        {
            SP_LevelAudioControl.instance.ToggleVoiceLines();
        }

        if (SP_SettingsPersistence.instance == null)
            Instantiate(settingsSaver);
        SP_SettingsPersistence.instance.SaveVoiceSettings(musicToggle.isOn);

    }

    public void LoadMainMenuScene()
    {
        OpenSettings(false);
        SceneManager.LoadScene(0);
    }


    public void ToggleSettings()
    {
        OpenSettings(!isSettingsOpen);
    }

    public void OpenSettings(bool open)
    {
        isSettingsOpen = open;
        settingsContainer.SetActive(open);
        settingsBG.SetActive(open);

        if (open)
        {
            if (musicToggle.isOn)
            {

                if (SP_PersistentAudio.instance != null && !SP_PersistentAudio.instance.isMusicOn)
                {
                    musicToggle.isOn = false;
                    ToggleMusic();
                }
            }


            if (soundToggle.isOn)
            {

                if (SP_LevelAudioControl.instance != null && !SP_LevelAudioControl.instance.isSoundOn)
                {
                    soundToggle.isOn = false;
                    SP_LevelAudioControl.instance.ToggleSound();
                }
            }

            if (voiceLineToggle.isOn)
            {

                if (SP_LevelAudioControl.instance != null && !SP_LevelAudioControl.instance.isVoiceAssistOn)
                {
                    voiceLineToggle.isOn = false;
                    SP_LevelAudioControl.instance.ToggleVoiceLines();
                }
            }
        }
    }
}
