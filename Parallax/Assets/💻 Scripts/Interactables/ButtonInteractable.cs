using UnityEngine;

public class ButtonInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private MonoBehaviour[] targets;

    public void Interact(GameObject interactor)
    {
        foreach (var target in targets)
        {
            if (target is IActivatable activatable)
            {
                activatable.Activate();
            }
        }

        Debug.Log("Button pressed!");
    }

    public string GetInteractText()
    {
        return "Press button";
    }
}