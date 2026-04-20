using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text promptText;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (promptText == null)
            promptText = GetComponent<TMP_Text>();

        if (canvasGroup == null || promptText == null)
        {
            Debug.LogError("InteractionPromptUI is missing references.", this);
            enabled = false;
            return;
        }

        canvasGroup.alpha = 0f;
        promptText.text = "";
    }

    public void Show(string message = "Press [E] to interact")
    {
        promptText.text = message;
        canvasGroup.alpha = 1f;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
    }
}