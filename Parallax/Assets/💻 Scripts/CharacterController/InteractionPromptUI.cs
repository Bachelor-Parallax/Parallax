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

        canvasGroup.alpha = 1f;
        promptText.text = "PROMPT TEST";
    }

    public void Show(string message = "Press [E] to interact")
    {
        Debug.Log("SHOW PROMPT: " + message);
        promptText.text = message;
        canvasGroup.alpha = 1f;
    }

    public void Hide()
    {
        Debug.Log("HIDE PROMPT");
        canvasGroup.alpha = 0f;
    }
}