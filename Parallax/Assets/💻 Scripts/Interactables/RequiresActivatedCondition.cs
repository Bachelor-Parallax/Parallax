using UnityEngine;

public class RequiresActivatedCondition : MonoBehaviour, IInteractCondition
{
    [SerializeField] private GameObject requiredObject;
    [SerializeField] private string failText = "Need to activate something first!";

    public string FailText => failText;

    public bool IsMet(GameObject interactor)
    {
        if (requiredObject == null) return false;

        IActivationState state = requiredObject.GetComponent<IActivationState>();

        return state != null && state.IsActivated;
    }
}