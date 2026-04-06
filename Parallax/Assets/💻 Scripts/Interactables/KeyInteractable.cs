using Unity.Netcode;
using UnityEngine;

public class KeyInteractable : NetworkBehaviour, ICatInteractable
{
    [SerializeField] private string keyId = "ButtonKey";

    public NetworkVariable<bool> keyCollected = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public void Interact(GameObject interactor)
    {
        if (!IsSpawned) return;

        NetworkObject interactorNetObj = interactor.GetComponent<NetworkObject>();
        if (interactorNetObj == null) return;

        CollectKeyServerRpc(interactorNetObj.OwnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CollectKeyServerRpc(ulong senderClientId)
    {
        if (keyCollected.Value) return;

        keyCollected.Value = true;
        Debug.Log($"Picked up key: {keyId}");

        NetworkObject.Despawn(true);
    }

    public string GetInteractText()
    {
        return "Pick up key";
    }
}