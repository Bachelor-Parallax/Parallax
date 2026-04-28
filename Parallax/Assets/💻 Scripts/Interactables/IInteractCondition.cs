using UnityEngine;

public interface IInteractCondition
{
    bool IsMet(GameObject interactor);
    string FailText { get; }
}