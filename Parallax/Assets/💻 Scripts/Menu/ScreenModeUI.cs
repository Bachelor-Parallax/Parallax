using TMPro;
using UnityEngine;

public class ScreenModeUI : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    public void OnScreenModeChanged(int index)
    {
        SettingsManager.Instance.SetScreenMode(index);
    }

    void OnEnable()
    {
        int mode = PlayerPrefs.GetInt("screen_mode", 0);
        dropdown.SetValueWithoutNotify(mode);
    }
}