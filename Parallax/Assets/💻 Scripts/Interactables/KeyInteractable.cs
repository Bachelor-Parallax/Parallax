using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class KeyInteractable : NetworkBehaviour, IInteractable
{
    [SerializeField] private string keyId = "ButtonKey";
    [SerializeField] private AudioClip keySound;

    private AudioSource _audioSource;
    private Rigidbody _rb;

    private Transform _holder;
    private Transform _holdPoint;
    
    private Collider _keyCollider;
    private Collider[] _playerColliders;

    private bool _isHeld;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _keyCollider = GetComponent<Collider>();
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
            if (_isHeld)
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

        _holdPoint = carry.MouthPoint;
        _isHeld = true;

        _rb.useGravity = false;
        _rb.isKinematic = true;

        _playerColliders = player.GetComponentsInChildren<Collider>();

        foreach (Collider col in _playerColliders)
            Physics.IgnoreCollision(_keyCollider, col, true);

        if (keySound && _audioSource)
            _audioSource.PlayOneShot(keySound);
    }

    private void Drop()
    {
        _isHeld = false;

        if (_playerColliders != null)
        {
            foreach (Collider col in _playerColliders)
                Physics.IgnoreCollision(_keyCollider, col, false);
        }
        
        _holdPoint = null;

        _rb.useGravity = true;
        _rb.isKinematic = false;
    }
    #endregion
    
    #region Human Interaction
    private void CollectKey(ulong senderClientId)
    {
        if (keySound != null && _audioSource != null)
        {
            _audioSource.pitch = Random.Range(0.9f, 1.1f);
            _audioSource.PlayOneShot(keySound);
        }

        Debug.Log($"Picked up key: {keyId} by client {senderClientId}");

        NetworkObject.Despawn(true);
    }
    #endregion

    private void LateUpdate()
    {
        if (!_isHeld || _holdPoint == null) return;

        transform.position = _holdPoint.position;
        transform.rotation = _holdPoint.rotation;
    }

    public string GetInteractText()
    {
        return _isHeld ? "Drop key" : "Pick up key";
    }
}