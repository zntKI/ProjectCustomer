using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
    AudioMixer audioMixer;

    [Header("SFX")]
    [Space]

    [SerializeField]
    AudioSource audioSourceBackground;
    [SerializeField]
    AudioSource audioSourceSFX;
    [Space]

    [SerializeField]
    AudioClip engineStartUpSound;
    [SerializeField]
    AudioClip tireSqueekingSound;
    [SerializeField]
    AudioClip[] honkSounds;
    [SerializeField]
    AudioClip policeSiren; 
    [SerializeField]
    AudioClip carCrash;
    [SerializeField]
    AudioClip carCrashShort;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        UIManager.OnVolumeChanged += ChangeVolume;

        DrunkAIMovement.OnStartCarPlaySound += PlayEngineStartUpSound;
        DrunkAIMovement.OnSwervePlaySound += PlaySwerveSound;
        GameManager.OnPlaySiren += PlayPoliceSirenSound;
        GameManager.OnPlayCrash += PlayCrashSound;
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

    void PlayCrashSound()
    {
        audioSourceBackground.Stop();
        audioSourceSFX.clip = carCrashShort;
        audioSourceSFX.Play();
    }

    void PlayPoliceSirenSound()
    {
        audioSourceBackground.clip = policeSiren;
        audioSourceBackground.loop = true;
        audioSourceBackground.Play();
    }

    void ChangeVolume(string volumeParamName, float value)
    {
        audioMixer.SetFloat(volumeParamName, value);
    }

    public bool IsSFXOn()
    {
        return audioSourceSFX.isPlaying;
    }

    void OnDestroy()
    {
        UIManager.OnVolumeChanged -= ChangeVolume;

        DrunkAIMovement.OnStartCarPlaySound -= PlayEngineStartUpSound;
        DrunkAIMovement.OnSwervePlaySound -= PlaySwerveSound;
        GameManager.OnPlaySiren -= PlayPoliceSirenSound;
        GameManager.OnPlayCrash += PlayCrashSound;
    }
}
