using Unity.Netcode;
using UnityEngine;

public class KeyInteractable : NetworkBehaviour, IInteractable
{
    [SerializeField] private string keyId = "ButtonKey";

    public NetworkVariable<bool> keyCollected = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private void OnTriggerEnter(Collider other)
    {
        if (!IsSpawned) return;
        if (keyCollected.Value) return;

        // Tjek om det er en player
        NetworkObject playerNetObj = other.GetComponentInParent<NetworkObject>();
        if (playerNetObj == null) return;

        // Optional: tjek at det faktisk er en spiller
        if (!other.GetComponentInParent<PlayerInteraction>()) return;

        CollectKeyServerRpc(playerNetObj.OwnerClientId);
    }

    public void Interact(GameObject interactor)
    {
        if (!IsSpawned) return;
        if (keyCollected.Value) return;

        NetworkObject interactorNetObj = interactor.GetComponent<NetworkObject>();
        if (interactorNetObj == null) return;

        CollectKeyServerRpc(interactorNetObj.OwnerClientId);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void CollectKeyServerRpc(ulong senderClientId)
    {
        if (keyCollected.Value) return;

        keyCollected.Value = true;
        Debug.Log($"Picked up key: {keyId} by client {senderClientId}");

        NetworkObject.Despawn(true);
    }

    public string GetInteractText()
    {
        return "Pick up key";
    }
}