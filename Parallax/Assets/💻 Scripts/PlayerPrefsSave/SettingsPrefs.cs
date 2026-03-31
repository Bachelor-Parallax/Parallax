using UnityEngine;

public static class SettingsPrefs
{
    // Keys
    private const string Key_Volume = "settings_volume";
    private const string Key_Muted = "settings_muted";
    private const string Key_ResWidth = "settings_resolution_width";
    private const string Key_ResHeight = "settings_resolution_height";
    private const string Key_DisplayMode = "settings_display_mode"; // stores (int)FullScreenMode
    private const string Key_MouseSens = "settings_mouse_sensitivity";

    // Defaults
    private const float DefaultVolume = 1f;
    private const int DefaultWidth = 1920;
    private const int DefaultHeight = 1080;
    private const FullScreenMode DefaultMode = FullScreenMode.FullScreenWindow; // borderless
    private const float DefaultMouseSens = 1f;

    // ---------- AUDIO ----------
    public static float Volume
    {
        get => Mathf.Clamp01(PlayerPrefs.GetFloat(Key_Volume, DefaultVolume));
        set
        {
            PlayerPrefs.SetFloat(Key_Volume, Mathf.Clamp01(value));
            PlayerPrefs.Save();
        }
    }

    public static bool Muted
    {
        get => PlayerPrefs.GetInt(Key_Muted, 0) == 1;
        set
        {
            PlayerPrefs.SetInt(Key_Muted, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    // ---------- MOUSE ----------
    public static float MouseSensitivity
    {
        get => Mathf.Max(0.01f, PlayerPrefs.GetFloat(Key_MouseSens, DefaultMouseSens));
        set
        {
            PlayerPrefs.SetFloat(Key_MouseSens, Mathf.Max(0.01f, value));
            PlayerPrefs.Save();
        }
    }

    // ---------- DISPLAY ----------
    public static Vector2Int Resolution
    {
        get
        {
            int width = Mathf.Max(320, PlayerPrefs.GetInt(Key_ResWidth, DefaultWidth));
            int height = Mathf.Max(200, PlayerPrefs.GetInt(Key_ResHeight, DefaultHeight));
            return new Vector2Int(width, height);
        }
        set
        {
            PlayerPrefs.SetInt(Key_ResWidth, Mathf.Max(320, value.x));
            PlayerPrefs.SetInt(Key_ResHeight, Mathf.Max(200, value.y));
            PlayerPrefs.Save();
        }
    }

    // Enum casting: store FullScreenMode as int
    public static FullScreenMode DisplayMode
    {
        get => (FullScreenMode)PlayerPrefs.GetInt(Key_DisplayMode, (int)DefaultMode);
        set
        {
            PlayerPrefs.SetInt(Key_DisplayMode, (int)value);
            PlayerPrefs.Save();
        }
    }
}