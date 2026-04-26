using Unity.Netcode;
using UnityEngine;

public class ButtonInteractable : NetworkBehaviour, IInteractable, IActivationState
{
    [SerializeField] private MonoBehaviour[] targets;
    [SerializeField] private KeyInteractable requiredKey;
    [SerializeField] private AudioClip buttonSound;

    private IInteractCondition[] conditions;
    private AudioSource audioSource;

    public bool IsActivated => isActivated.Value;

    private NetworkVariable<bool> isActivated = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        conditions = GetComponents<IInteractCondition>();
    }

    public bool CanInteract(GameObject interactor)
    {
        RoleController role = interactor.GetComponent<RoleController>();

        if (role == null || !role.IsHuman)
            return false;

        if (requiredKey != null && !requiredKey.IsCollected)
            return false;

        foreach (IInteractCondition condition in conditions)
        {
            if (!condition.IsMet(interactor))
                return false;
        }

        return true;
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
        NetworkObject playerObj = NetworkManager.Singleton.ConnectedClients[senderClientId].PlayerObject;
        if (playerObj == null) return;

        if (!CanInteract(playerObj.gameObject))
        {
            Debug.Log("Button locked - conditions not met.");
            return;
        }

        isActivated.Value = true;

        ActivateTargets();
        PlayButtonSoundClientRpc();

        Debug.Log($"Button pressed by client {senderClientId}");
    }

    private void ActivateTargets()
    {
        foreach (MonoBehaviour target in targets)
        {
            if (target == null) continue;

            if (target is IActivatable activatable)
                activatable.Activate();
            else
                Debug.LogWarning($"{target.name} does not implement IActivatable.");
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

    public string GetFailText(GameObject interactor)
    {
        RoleController role = interactor.GetComponent<RoleController>();

        foreach (IInteractCondition condition in conditions)
        {
            if (!condition.IsMet(interactor))
                return condition.FailText;
        }

        return "";
    }

    public string GetInteractText()
    {
        return "Press button [E]";
    }
}