using UnityEngine;
using UnityEngine.UI;

public class SettingsUIBinder : MonoBehaviour
{
    [Header("Audio")]
    public Slider masterVolumeSlider;
    public Toggle muteToggle;

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

        // INITIAL SYNC - Get settings from PlayerPrefs and apply the setting to the UI
        SyncAll();
    }

    private void OnDisable()
    {
        var sm = SettingsManager.Instance;
        // UN-Subscribe to events
        sm.OnMasterVolumeChanged -= UpdateVolume;
        sm.OnMuteChanged -= UpdateMute;
        sm.OnCamaraSensitivityChanged -= UpdateSensitivity;
        sm.OnConfineMouseChanged -= UpdateConfineMouse;
    }

    // -------------------------
    // INITIAL LOAD - Apply all the settings from PlayerPrefs
    // -------------------------
    void SyncAll()
    {
        var sm = SettingsManager.Instance;
        UpdateVolume(sm.GetMasterVolume());
        UpdateMute(sm.GetMute());
        UpdateSensitivity(sm.GetMouseSensitivity());
        UpdateConfineMouse(sm.GetConfineMouse());
    }

    // -------------------------
    // UPDATE METHODS
    // -------------------------
    void UpdateVolume(float value)
    {
        masterVolumeSlider.SetValueWithoutNotify(value);
    }
    void UpdateMute(bool value)
    {
        muteToggle.SetIsOnWithoutNotify(value);
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