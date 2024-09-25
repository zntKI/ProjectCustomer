using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    Slider masterVolSlider, musicVolSlider, sfxVolumeSlider;

    public static event Action<string, float> OnVolumeChanged;

    public void ChangeMasterVol()
    {
        OnVolumeChanged?.Invoke("MasterVol", masterVolSlider.value);
    }
    public void ChangeMusicVol()
    {
        OnVolumeChanged?.Invoke("MusicVol", musicVolSlider.value);
    }
    public void ChangeSfxVol()
    {
        OnVolumeChanged?.Invoke("SFXVol", sfxVolumeSlider.value);
    }
}
