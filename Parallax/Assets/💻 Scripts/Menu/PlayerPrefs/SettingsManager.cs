using System;
using UnityEngine;
using UnityEngine.AI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    // EVENTS
    public event Action<float> OnMasterVolumeChanged;
    public event Action<bool> OnMuteChanged;
    public event Action<float> OnMouseSensitivityChanged;
    public event Action<string> OnResolutionChanged;
    public event Action<FullScreenMode> OnScreenModeChanged;
    public event Action<bool> OnHideMouseChanged;
    public event Action<bool> OnConfineMouseChanged;

    // PlayerPrefs keys
    private const string MasterVolumeKey = "master_volume";
    private const string MuteKey = "mute_all";
    private const string SensitivityKey = "mouse_sensitivity";
    private const string ResolutionKey = "resolution_string";
    private const string ScreenModeKey = "screen_mode";
    private const string HideMouseKey = "hide_mouse";
    private const string ConfineMouseKey = "confine_mouse";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAll();
    }

    // -------------------------
    // SETTERS (UI calls these)
    // -------------------------

    public void SetMasterVolume(float value)
    {
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
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

    public void SetMouseSensitivity(float value)
    {
        PlayerPrefs.SetFloat(SensitivityKey, value);
        PlayerPrefs.Save();

        OnMouseSensitivityChanged?.Invoke(value);
    }

    public void SetScreenMode(int index)
    {
        PlayerPrefs.SetInt(ScreenModeKey, index);
        PlayerPrefs.Save();

        ApplyScreenMode();
    }

    public void SetResolution(string resolution)
    {
        PlayerPrefs.SetString(ResolutionKey, resolution);
        PlayerPrefs.Save();

        ApplyResolution(); // will only apply if windowed
    }

    public void ToggleHideMouse()
    {
        bool value = !GetHideMouse();

        PlayerPrefs.SetInt(HideMouseKey, value ? 1 : 0);
        PlayerPrefs.Save();

        ApplyCursor();
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

    private void ApplyScreenMode()
    {
        int index = PlayerPrefs.GetInt(ScreenModeKey, 0);

        FullScreenMode mode = FullScreenMode.Windowed;

        switch (index)
        {
            case 0: mode = FullScreenMode.Windowed; break;
            case 1: mode = FullScreenMode.ExclusiveFullScreen; break;
        }

        Screen.fullScreenMode = mode;

        if (mode == FullScreenMode.ExclusiveFullScreen)
        {
            // FULLSCREEN OVERRIDES RESOLUTION
            Resolution native = Screen.currentResolution;
            Screen.SetResolution(native.width, native.height, mode);
        }
        else
        {
            // When returning to windowed → apply saved resolution
            ApplyResolution();
        }

        OnScreenModeChanged?.Invoke(mode);
    }

    private void ApplyResolution()
    {
        // Ignore resolution changes if fullscreen
        if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
            return;

        string resolution = PlayerPrefs.GetString(ResolutionKey, "1920x1080");

        // Optional: allow "1920 x 1080"
        resolution = resolution.Replace(" ", "");

        string[] parts = resolution.Split('x');

        if (parts.Length != 2)
        {
            Debug.LogError("Invalid resolution format: " + resolution);
            return;
        }

        if (int.TryParse(parts[0], out int width) &&
            int.TryParse(parts[1], out int height))
        {
            Screen.SetResolution(width, height, Screen.fullScreenMode);
            OnResolutionChanged?.Invoke(resolution);
        }
        else
        {
            Debug.LogError("Failed to parse resolution: " + resolution);
        }
    }

    private void ApplyCursor()
    {
        bool hide = GetHideMouse();
        bool confine = GetConfineMouse();

        Cursor.visible = !hide;
        Cursor.lockState = confine ? CursorLockMode.Confined : CursorLockMode.None;

        OnHideMouseChanged?.Invoke(hide);
        OnConfineMouseChanged?.Invoke(confine);
    }

    // -------------------------
    // LOAD
    // -------------------------
    private void LoadAll()
    {
        ApplyScreenMode();     // MUST come first
        ApplyVolume();
        ApplyCursor();

        float sensitivity = GetMouseSensitivity();
        OnMouseSensitivityChanged?.Invoke(sensitivity);
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

    public bool GetHideMouse() =>
        PlayerPrefs.GetInt(HideMouseKey, 0) == 1;

    public bool GetConfineMouse() =>
        PlayerPrefs.GetInt(ConfineMouseKey, 0) == 1;

    public string GetResolution() =>
        PlayerPrefs.GetString(ResolutionKey, "1920x1080");
}