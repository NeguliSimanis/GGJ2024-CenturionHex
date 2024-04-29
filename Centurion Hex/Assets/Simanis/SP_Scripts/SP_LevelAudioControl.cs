using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_LevelAudioControl : MonoBehaviour
{
    public static SP_LevelAudioControl instance;

    [Header("EVA Voice lines")]
    public AudioClip insuffcientSpeedSFX;
    public AudioClip outOfRangeSFX;
    public AudioClip sleepingSFX;
    public AudioClip civilUnitCannotAttackSFX;
    public AudioClip unitLostSFX;
    public AudioClip yourTurnSFX;
    public AudioClip enemyTurnSFX;
    public AudioClip cannot_command_enemy_unit_sfx;

    [Header("Other sfx")]
    public AudioClip landmine_explosion_sfx;

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
        audioSource.pitch = 1f;
        voiceLineAudioSource.PlayOneShot(insuffcientSpeedSFX);
    }

    public void PlaySFX(AudioClip sfx, bool isVoiceLine, float volumeMultiplier = 1f)
    {
        audioSource.pitch = 1f;

        if (!isVoiceLine)
            audioSource.PlayOneShot(sfx, volumeMultiplier);
        else
        {
            voiceLineAudioSource.volume = volumeMultiplier;
            voiceLineAudioSource.clip = sfx;
            voiceLineAudioSource.Play();
        }
    }

    public void PlayAttackSFX()
    {
        int randomRoll = Random.Range(0, attackSFXs.Length);
        audioSource.pitch = attackPitch;
        audioSource.PlayOneShot(attackSFXs[randomRoll]);
    }

    public void PlayMoveSFX()
    {
        int randomRoll = Random.Range(0, moveSFXs.Length);
        audioSource.PlayOneShot(moveSFXs[randomRoll]);
    }
}
