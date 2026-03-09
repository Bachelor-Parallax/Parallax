using TMPro;
using UnityEngine;

public class StatusUI : MonoBehaviour
{
    public static StatusUI Instance;

    [SerializeField] private TextMeshProUGUI statusText;

    private void Awake()
    {
        Instance = this;
    }

    public void Show(string message)
    {
        statusText.text = message;
    }

    public void Clear()
    {
        statusText.text = "";
    }
}