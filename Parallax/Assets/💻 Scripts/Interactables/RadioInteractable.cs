using System;
using UnityEngine;

public class RadioInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private MusicManager musicManager;

    public void Interact(GameObject interactor)
    {
        musicManager.TogglePause();
    }

    public bool CanInteract(GameObject interactor)
    {
        RoleController role = interactor.GetComponent<RoleController>();
        if (role == null || !role.IsHuman)
        {
            return false;
        }
        return true;
    }

    public string GetInteractText()
    {
        return "Press [E] to toggle music";
    }
}