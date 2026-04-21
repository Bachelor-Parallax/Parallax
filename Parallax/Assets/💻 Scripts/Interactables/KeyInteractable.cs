using Unity.Netcode;
using UnityEngine;

public class KeyInteractable : NetworkBehaviour, IInteractable
{
    [SerializeField] private string keyId = "ButtonKey";
    [SerializeField] private AudioClip keySound;
    private AudioSource audioSource;

    public NetworkVariable<bool> keyCollected = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public bool IsCollected => keyCollected.Value;

    private Collider keyCollider;
    private Renderer[] keyRenderers;

    private void Awake()
    {
        keyCollider = GetComponent<Collider>();
        keyRenderers = GetComponentsInChildren<Renderer>(true);
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsSpawned) return;
        if (keyCollected.Value) return;

        NetworkObject playerNetObj = other.GetComponentInParent<NetworkObject>();
        if (playerNetObj == null) return;

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

        if (keySound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(keySound);
        }

        keyCollected.Value = true;
        Debug.Log($"Picked up key: {keyId} by client {senderClientId}");

        HideKeyClientRpc();
        NetworkObject.Despawn(false);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void HideKeyClientRpc()
    {
        if (keyCollider != null)
            keyCollider.enabled = false;

        foreach (Renderer r in keyRenderers)
        {
            r.enabled = false;
        }
    }

    public string GetInteractText()
    {
        return "Pick up key";
    }
}