using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubscriberEXAMPLES : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [Header("Optional Systems")]
    [SerializeField] private Camera playerCamera;

    private float currentSensitivity = 1f;

    // -------------------------
    // SUBSCRIBE
    // -------------------------
    void OnEnable()
    {
        var settings = SettingsManager.Instance;

        if (settings == null) return;

        settings.OnMasterVolumeChanged += HandleVolumeChanged;
        settings.OnMuteChanged += HandleMuteChanged;
        settings.OnMouseSensitivityChanged += HandleSensitivityChanged;
        //settings.OnResolutionChanged += HandleResolutionChanged;
        settings.OnHideMouseChanged += HandleHideMouseChanged;
        settings.OnConfineMouseChanged += HandleConfineMouseChanged;

        // Initial sync (important)
        SyncFromSettings();
    }

    // -------------------------
    // UNSUBSCRIBE
    // -------------------------
    void OnDisable()
    {
        var settings = SettingsManager.Instance;

        if (settings == null) return;

        settings.OnMasterVolumeChanged -= HandleVolumeChanged;
        settings.OnMuteChanged -= HandleMuteChanged;
        settings.OnMouseSensitivityChanged -= HandleSensitivityChanged;
        //settings.OnResolutionChanged -= HandleResolutionChanged;
        settings.OnHideMouseChanged -= HandleHideMouseChanged;
        settings.OnConfineMouseChanged -= HandleConfineMouseChanged;
    }

    // -------------------------
    // INITIAL SYNC
    // -------------------------
    void SyncFromSettings()
    {
        var settings = SettingsManager.Instance;

        // UI elements
        if (volumeSlider != null)
            volumeSlider.SetValueWithoutNotify(settings.GetMasterVolume());

        if (sensitivitySlider != null)
            sensitivitySlider.SetValueWithoutNotify(settings.GetMouseSensitivity());

        // Cursor + audio already applied globally by SettingsManager
    }

    // -------------------------
    // EVENT HANDLERS
    // -------------------------

    void HandleVolumeChanged(float value)
    {
        if (volumeSlider != null)
            volumeSlider.SetValueWithoutNotify(value);

        Debug.Log("Volume updated: " + value);
    }

    void HandleMuteChanged(bool muted)
    {
        Debug.Log("Mute state: " + muted);
    }

    void HandleSensitivityChanged(float value)
    {
        currentSensitivity = value;

        if (sensitivitySlider != null)
            sensitivitySlider.SetValueWithoutNotify(value);
    }

    void HandleResolutionChanged(int width, int height, FullScreenMode mode)
    {
        Debug.Log($"Resolution: {width}x{height}, Mode: {mode}");

        // Optional: update dropdown if needed
        // (only if your dropdown reflects current state)
    }

    void HandleHideMouseChanged(bool hide)
    {
        Debug.Log("Hide mouse: " + hide);
    }

    void HandleConfineMouseChanged(bool confine)
    {
        Debug.Log("Confine mouse: " + confine);
    }

    // -------------------------
    // EXAMPLE USAGE (camera)
    // -------------------------
    void Update()
    {
        if (playerCamera == null) return;

        float mouseX = Input.GetAxis("Mouse X") * currentSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * currentSensitivity;

        playerCamera.transform.Rotate(-mouseY, mouseX, 0f);
    }
}
