using Unity.Netcode;
using UnityEngine;

public class ButtonInteractable : NetworkBehaviour, IInteractable
{
    [SerializeField] private MovingPlatform[] targets;
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
        if (requiredKey != null && !requiredKey.IsCollected)
        {
            Debug.Log("Button locked - missing key.");
            return;
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
        if (requiredKey != null && !requiredKey.IsCollected)
            return "Button Locked - Missing Key";

        return "Press button [E]";
    }
}