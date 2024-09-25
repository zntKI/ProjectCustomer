using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioMixer audioMixer;

    [Header("SFX")]
    [Space]

    [SerializeField]
    AudioSource audioSourceSFX;
    [Space]

    [SerializeField]
    AudioClip engineStartUpSound;
    [SerializeField]
    AudioClip tireSqueekingSound;
    [SerializeField]
    AudioClip[] honkSounds;

    void Awake()
    {
        UIManager.OnVolumeChanged += ChangeVolume;

        DrunkAIMovement.OnStartCarPlaySound += PlayEngineStartUpSound;
        DrunkAIMovement.OnSwervePlaySound += PlaySwerveSound;
    }

    void PlayEngineStartUpSound()
    {
        audioSourceSFX.clip = engineStartUpSound;
        audioSourceSFX.Play();
    }

    void PlaySwerveSound()
    {
        audioSourceSFX.clip = tireSqueekingSound;
        audioSourceSFX.Play();
    }

    void ChangeVolume(string volumeParamName, float value)
    {
        audioMixer.SetFloat(volumeParamName, value);
    }

    void OnDestroy()
    {
        UIManager.OnVolumeChanged -= ChangeVolume;

        DrunkAIMovement.OnStartCarPlaySound -= PlayEngineStartUpSound;
        DrunkAIMovement.OnSwervePlaySound -= PlaySwerveSound;
    }
}
