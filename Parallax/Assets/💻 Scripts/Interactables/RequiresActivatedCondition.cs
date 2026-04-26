using UnityEngine;

public class RequiresActivatedCondition : MonoBehaviour, IInteractCondition
{
    [SerializeField] private MonoBehaviour requiredObject;
    [SerializeField] private string failText = "Need to activate something first!";

    public string FailText => failText;

    public bool IsMet(GameObject interactor)
    {
        return requiredObject is IActivationState state && state.IsActivated;
    }
}