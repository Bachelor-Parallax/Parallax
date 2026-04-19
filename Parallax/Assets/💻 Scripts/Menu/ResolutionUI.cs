using TMPro;
using UnityEngine;

public class ResolutionUI : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    public void OnResolutionChanged(int index)
    {
        string value = dropdown.options[index].text;
        SettingsManager.Instance.SetResolution(value);
    }

    void OnEnable()
    {
        dropdown.SetValueWithoutNotify(GetSavedIndex());
    }

    int GetSavedIndex()
    {
        string saved = SettingsManager.Instance.GetResolution();

        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == saved)
                return i;
        }

        return 0;
    }
}