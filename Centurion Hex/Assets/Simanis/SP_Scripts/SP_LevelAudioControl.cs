using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_LevelAudioControl : MonoBehaviour
{
    public static SP_LevelAudioControl instance;

    [Header("EVA Voice lines")]
    public bool isVoiceAssistOn = true;
    public AudioClip insuffcientSpeedSFX;
    public AudioClip outOfRangeSFX;
    public AudioClip sleepingSFX;
    public AudioClip civilUnitCannotAttackSFX;
    public AudioClip unitLostSFX;
    public AudioClip yourTurnSFX;
    public AudioClip enemyTurnSFX;
    public AudioClip cannot_command_enemy_unit_sfx;

    [Header("Other sfx")]
    public bool isSoundOn = true;
    public AudioClip landmine_explosion_sfx;
    public AudioClip jewel_pickup;
    public AudioClip victory_sfx;

    [Header("move sfx")]
    public AudioClip[] moveSFXs;

    [Header("attack sfx")]
    public AudioClip[] attackSFXs;
    public float attackPitch = 1.3f;

    private AudioSource audioSource;
    public AudioSource voiceLineAudioSource;

    private void Awake()
    {
        instance = this;
        audioSource = gameObject.GetComponent<AudioSource>();
    }
    
    public void PlayInsuffcientSpeedSFX()
    {
        if (!isVoiceAssistOn)
            return;
        audioSource.pitch = 1f;
        voiceLineAudioSource.PlayOneShot(insuffcientSpeedSFX);
    }

    public void ToggleSound()
    {
        Debug.Log("sfx toggled");
        isSoundOn = !isSoundOn;
    }

    public void ToggleVoiceLines()
    {
        isVoiceAssistOn = !isVoiceAssistOn;
    }

    public void PlaySFX(AudioClip sfx, bool isVoiceLine, float volumeMultiplier = 1f)
    {
        audioSource.pitch = 1f;

        if (!isVoiceLine)
        {
            if (isSoundOn)
                audioSource.PlayOneShot(sfx, volumeMultiplier);
        }
        else if (isVoiceAssistOn)
        {
            voiceLineAudioSource.volume = volumeMultiplier;
            voiceLineAudioSource.clip = sfx;
            voiceLineAudioSource.Play();
        }
    }

    public void PlayAttackSFX()
    {
        if (!isSoundOn)
            return;
        int randomRoll = Random.Range(0, attackSFXs.Length);
        audioSource.pitch = attackPitch;
        audioSource.PlayOneShot(attackSFXs[randomRoll]);
    }

    public void PlayMoveSFX()
    {
        if (!isSoundOn)
            return;
        int randomRoll = Random.Range(0, moveSFXs.Length);
        audioSource.PlayOneShot(moveSFXs[randomRoll]);
    }
}
