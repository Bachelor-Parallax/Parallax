using UnityEngine;

public abstract class ConditionalInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private IInteractCondition[] conditions;

    private void Awake()
    {
        conditions = GetComponents<IInteractCondition>();
    }
    
    public bool CanInteract(GameObject interactor)
    {
        foreach (var condition in conditions)
        {
            if (condition is IInteractCondition interactCondition)
            {
                if (!interactCondition.IsMet(interactor))
                    return false;
            }
        }

        return true;
    }

    public string GetInteractText()
    {
        return "Interact";
    }
    public void Interact(GameObject interactor)
    {
        if (!CanInteract(interactor))
        {
            Debug.Log("Cannot interact yet.");
            return;
        }

        OnInteract(interactor);
    }

    protected abstract void OnInteract(GameObject interactor);
}