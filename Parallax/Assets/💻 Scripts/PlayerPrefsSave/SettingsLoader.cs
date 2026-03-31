using UnityEngine;

public class SettingsLoader : MonoBehaviour
{
    void Start()
    {
        ApplyAll();
    }

    public static void ApplyAll()
    {
        // Audio
        AudioListener.volume = SettingsPrefs.Muted ? 0f : SettingsPrefs.Volume;

        // Display
        Vector2Int res = SettingsPrefs.Resolution;
        Screen.SetResolution(res.x, res.y, SettingsPrefs.DisplayMode);
    }
}