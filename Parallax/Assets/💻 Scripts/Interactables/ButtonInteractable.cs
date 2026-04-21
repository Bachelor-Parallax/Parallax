using Unity.Netcode;
using UnityEngine;

public class ButtonInteractable : NetworkBehaviour, IInteractable
{
    [SerializeField] private MonoBehaviour[] targets;
    [SerializeField] private KeyInteractable requiredKey;
    [SerializeField] private AudioClip buttonSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public bool CanInteract(GameObject interactor)
    {
        RoleController role = interactor.GetComponent<RoleController>();
        return role != null && role.IsHuman;
    }

    public void Interact(GameObject interactor)
    {
        NetworkObject netObj = interactor.GetComponent<NetworkObject>();
        if (netObj == null) return;

        PressButtonServerRpc(netObj.OwnerClientId);
    }

    [Rpc(SendTo.Server)]
    private void PressButtonServerRpc(ulong senderClientId)
    {
        // Optional key check
        if (requiredKey != null)
        {
            if (requiredKey == null)
            {
                Debug.Log("Button requires a key, but key is missing.");
                return;
            }
        }

        ActivateTargets();

        PlayButtonSoundClientRpc();

        Debug.Log($"Button pressed by client {senderClientId}");
    }

    private void ActivateTargets()
    {
        foreach (var target in targets)
        {
            if (target is IActivatable activatable)
            {
                activatable.Activate();
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayButtonSoundClientRpc()
    {
        if (buttonSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(buttonSound);
        }
    }

    public string GetInteractText()
    {
        return requiredKey != null
            ? "Button Locked - Missing Key"
            : "Press button [E]";
    }
}