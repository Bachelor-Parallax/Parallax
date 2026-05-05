using UnityEngine;
using UnityEngine.UI;

public class SettingsUIBinderV2 : MonoBehaviour
{
    [Header("Audio")]
    public Slider masterVolumeSlider;
    public Toggle muteToggle;

    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider dialogueVolumeSlider;

    [Header("Mouse")]
    public Slider sensitivitySlider;
    public Toggle confineMouseToggle;

    private void OnEnable()
    {
        var sm = SettingsManager.Instance;

        // Subscribe to events
        sm.OnMasterVolumeChanged += UpdateVolume;
        sm.OnMuteChanged += UpdateMute;
        sm.OnCamaraSensitivityChanged += UpdateSensitivity;
        sm.OnConfineMouseChanged += UpdateConfineMouse;
        
        sm.OnMusicVolumeChanged += UpdateMusicVolume;
        sm.OnSFXVolumeChanged += UpdateSFXVolume;
        sm.OnDialogueVolumeChanged += UpdateDialogueVolume;

        // INITIAL SYNC
        SyncAll();
    }

    private void OnDisable()
    {
        var sm = SettingsManager.Instance;

        sm.OnMasterVolumeChanged -= UpdateVolume;
        sm.OnMuteChanged -= UpdateMute;
        sm.OnCamaraSensitivityChanged -= UpdateSensitivity;
        sm.OnConfineMouseChanged -= UpdateConfineMouse;

        sm.OnMusicVolumeChanged -= UpdateMusicVolume;
        sm.OnSFXVolumeChanged -= UpdateSFXVolume;
        sm.OnDialogueVolumeChanged -= UpdateDialogueVolume;
    }
    
    // INITIAL LOAD
    void SyncAll()
    {
        var sm = SettingsManager.Instance;

        UpdateVolume(sm.GetMasterVolume());
        UpdateMute(sm.GetMute());
        UpdateSensitivity(sm.GetMouseSensitivity());
        UpdateConfineMouse(sm.GetConfineMouse());

        UpdateMusicVolume(sm.GetMusicVolume());
        UpdateSFXVolume(sm.GetSFXVolume());
        UpdateDialogueVolume(sm.GetDialogueVolume());

    }
    
    
    // UPDATE METHODS
    void UpdateVolume(float value)
    {
        masterVolumeSlider.SetValueWithoutNotify(value);
    }
    void UpdateMute(bool value)
    {
        muteToggle.SetIsOnWithoutNotify(value);
    }

    void UpdateMusicVolume(float value)
    {
        musicVolumeSlider.SetValueWithoutNotify(value);
    }
    void UpdateSFXVolume(float value)
    {
        sfxVolumeSlider.SetValueWithoutNotify(value);
    }

    void UpdateDialogueVolume(float value)
    {
        dialogueVolumeSlider.SetValueWithoutNotify(value);
    }

    void UpdateSensitivity(float value)
    {
        sensitivitySlider.SetValueWithoutNotify(value);
    }

    void UpdateConfineMouse(bool value)
    {
        confineMouseToggle.SetIsOnWithoutNotify(value);
    }
}