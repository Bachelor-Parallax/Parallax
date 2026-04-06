using UnityEngine;

public interface IHumanInteractable
{
    void Interact(GameObject interactor);
    string GetInteractText();
}

public interface ICatInteractable
{
    void Interact(GameObject interactor);
    string GetInteractText();
}