using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_LevelAudioControl : MonoBehaviour
{
    public static SP_LevelAudioControl instance;

    public AudioClip insuffcientSpeedSFX;
    public AudioClip outOfRangeSFX;
    public AudioClip sleepingSFX;
    public AudioClip civilUnitCannotAttackSFX;
    public AudioClip unitLostSFX;
    public AudioClip yourTurnSFX;
    public AudioClip enemyTurnSFX;
    public AudioClip cannot_command_enemy_unit_sfx;

    [Header("move sfx")]
    public AudioClip[] moveSFXs;

    [Header("attack sfx")]
    public AudioClip[] attackSFXs;
    public float attackPitch = 1.3f;

    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
        audioSource = gameObject.GetComponent<AudioSource>();
    }
    
    public void PlayInsuffcientSpeedSFX()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(insuffcientSpeedSFX);
    }

    public void PlaySFX(AudioClip sfx)
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(sfx);
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
