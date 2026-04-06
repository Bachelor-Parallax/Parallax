using Unity.Netcode;
using UnityEngine;

public class ButtonInteractable : NetworkBehaviour, IHumanInteractable
{
    [SerializeField] private MonoBehaviour[] targets;
    [SerializeField] private KeyInteractable requiredKey;

    public void Interact(GameObject interactor)
    {
        if (requiredKey == null || !requiredKey.keyCollected.Value)
        {
            Debug.Log("Button pressed, but key not collected!");
            return;
        }

        PressButtonServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PressButtonServerRpc()
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