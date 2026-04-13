using TMPro;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] GameObject loadingPanel;
    [SerializeField] TMP_Text loadingText;

    public void Show(string text)
    {
        loadingPanel.SetActive(true);
        loadingText.text = text;
    }

    public void Hide()
    {
        loadingPanel.SetActive(false);
    }
}