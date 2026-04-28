using System;
using UnityEngine;
using UnityEngine.UI;


public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    // EVENTS
    public event Action<float> OnMasterVolumeChanged;
    public event Action<bool> OnMuteChanged;
    public event Action<float> OnCamaraSensitivityChanged;
    public event Action<bool> OnConfineMouseChanged;

    // PlayerPrefs keys
    private const string MasterVolumeKey = "master_volume";
    private const string MuteKey = "mute_all";
    private const string SensitivityKey = "camara_sensitivity";
    private const string ConfineMouseKey = "confine_mouse";

    private void Awake()
    {
        LoadAll();
    }


    // SETTERS
    public void SetMasterVolume(Slider sliderValue)
    {
        PlayerPrefs.SetFloat(MasterVolumeKey, sliderValue.value);
        PlayerPrefs.Save();
        ApplyVolume();
    }

    public void ToggleMute()
    {
        bool muted = !GetMute();

        PlayerPrefs.SetInt(MuteKey, muted ? 1 : 0);
        PlayerPrefs.Save();

        ApplyVolume();
    }

    public void SetMouseSensitivity(Slider sliderValue)
    {
        PlayerPrefs.SetFloat(SensitivityKey, sliderValue.value);
        PlayerPrefs.Save();

        OnCamaraSensitivityChanged?.Invoke(sliderValue.value);
    }

    public void ToggleConfineMouse()
    {
        bool value = !GetConfineMouse();

        PlayerPrefs.SetInt(ConfineMouseKey, value ? 1 : 0);
        PlayerPrefs.Save();

        ApplyCursor();
    }

    // -------------------------
    // APPLY METHODS
    // -------------------------
    private void ApplyVolume()
    {
        float volume = GetMasterVolume();
        bool muted = GetMute();

        AudioListener.volume = muted ? 0f : volume;

        OnMasterVolumeChanged?.Invoke(volume);
        OnMuteChanged?.Invoke(muted);
    }

    private void ApplyCursor()
    {
        bool confine = GetConfineMouse();
        
        Cursor.lockState = confine ? CursorLockMode.Confined : CursorLockMode.None;
        
        OnConfineMouseChanged?.Invoke(confine);
    }

    // -------------------------
    // LOAD
    // -------------------------
    private void LoadAll()
    {
        ApplyVolume();
        ApplyCursor();

        float sensitivity = GetMouseSensitivity();
        OnCamaraSensitivityChanged?.Invoke(sensitivity);
    }

    // -------------------------
    // GETTERS
    // -------------------------
    public float GetMasterVolume() =>
        PlayerPrefs.GetFloat(MasterVolumeKey, 1f);

    public bool GetMute() =>
        PlayerPrefs.GetInt(MuteKey, 0) == 1;

    public float GetMouseSensitivity() =>
        PlayerPrefs.GetFloat(SensitivityKey, 1f);
    
    public bool GetConfineMouse() =>
        PlayerPrefs.GetInt(ConfineMouseKey, 0) == 1;
}