using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class KeyInteractable : NetworkBehaviour, IInteractable
{
    [SerializeField] private string keyId = "ButtonKey";
    [SerializeField] private AudioClip keySound;

    private AudioSource audioSource;
    private Rigidbody rb;

    private Transform holder;
    private Transform holdPoint;
    
    private Collider keyCollider;
    private Collider[] playerColliders;

    private bool isHeld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        keyCollider = GetComponent<Collider>();
    }

    public bool CanInteract(GameObject interactor)
    {
        return true;
    }

    public void Interact(GameObject interactor)
    {
        Debug.Log("Key Interact called, IsSpawned: " + IsSpawned);
        if (!IsSpawned) return;

        NetworkObject playerNetObj = interactor.GetComponent<NetworkObject>();
        if (playerNetObj == null) return;

        InteractServerRpc(playerNetObj.OwnerClientId);
    }
    
    [Rpc(SendTo.Server)]
    private void InteractServerRpc(ulong clientId)
    {
        NetworkObject playerObj =
            NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);

        if (playerObj == null) return;

        RoleController role = playerObj.GetComponent<RoleController>();
        if (role == null) return;

        if (role.IsCat)
        {
            if (isHeld)
                Drop();
            else
                Pickup(playerObj.gameObject);
        }
        else if (role.IsHuman)
        {
            CollectKey(clientId);
        }
    }

    #region Cat Interaction
    private void Pickup(GameObject player)
    {
        MouthCarryPoint carry = player.GetComponentInChildren<MouthCarryPoint>();
        if (carry == null) return;

        holdPoint = carry.MouthPoint;
        isHeld = true;

        rb.useGravity = false;
        rb.isKinematic = true;

        playerColliders = player.GetComponentsInChildren<Collider>();

        foreach (Collider col in playerColliders)
            Physics.IgnoreCollision(keyCollider, col, true);

        if (keySound && audioSource)
            audioSource.PlayOneShot(keySound);
    }

    private void Drop()
    {
        isHeld = false;

        if (playerColliders != null)
        {
            foreach (Collider col in playerColliders)
                Physics.IgnoreCollision(keyCollider, col, false);
        }
        
        holdPoint = null;

        rb.useGravity = true;
        rb.isKinematic = false;
    }
    #endregion
    
    #region Human Interaction
    private void CollectKey(ulong senderClientId)
    {
        if (keySound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(keySound);
        }

        Debug.Log($"Picked up key: {keyId} by client {senderClientId}");

        NetworkObject.Despawn(true);
    }
    #endregion

    private void LateUpdate()
    {
        if (!isHeld || holdPoint == null) return;

        transform.position = holdPoint.position;
        transform.rotation = holdPoint.rotation;
    }

    public string GetInteractText()
    {
        return isHeld ? "Drop key" : "Pick up key";
    }
}