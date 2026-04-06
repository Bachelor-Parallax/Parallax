using UnityEngine;

public class ButtonInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject targetObject;

    public void Interact(GameObject interactor)
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
            Debug.Log("Button pressed, target activated.");
        }
    }

    public string GetInteractText()
    {
        return "Press button";
    }
}