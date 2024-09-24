using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioMixer audioMixer;

    void Awake()
    {
        // Subscribe to UI change slider event
    }

    void ChangeVolume(string volumeParamName, float value)
    {
        audioMixer.SetFloat(volumeParamName, value);
    }

    void OnDestroy()
    {
        // Unsubscribe to UI change slider event
    }
}
