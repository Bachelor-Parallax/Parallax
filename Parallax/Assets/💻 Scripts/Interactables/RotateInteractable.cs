using Unity.Netcode;
using UnityEngine;

public class RotateInteractable : NetworkBehaviour, IInteractable
{
    [SerializeField] private float rotateAmountY = 90f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private AudioClip rotateSound;
    [SerializeField] private KeyInteractable requiredKey;

    private AudioSource audioSource;
    private Quaternion targetRotation;
    private bool isRotating;
    private bool hasRotated;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        targetRotation = transform.rotation;
    }

    public bool CanInteract(GameObject interactor)
    {
        if (hasRotated) return false;

        RoleController role = interactor.GetComponent<RoleController>();
        return role != null && role.IsHuman;
    }

    public void Interact(GameObject interactor)
    {
        if (hasRotated || isRotating) return;

        NetworkObject netObj = interactor.GetComponent<NetworkObject>();
        if (netObj == null) return;

        RotateServerRpc(netObj.OwnerClientId);
    }

    [Rpc(SendTo.Server)]
    private void RotateServerRpc(ulong senderClientId)
    {
        if (hasRotated || isRotating) return;

        if (requiredKey != null && !requiredKey.IsCollected)
        {
            Debug.Log("Rotation locked - missing key.");
            return;
        }

        hasRotated = true;

        Quaternion newTargetRotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y - rotateAmountY,
            transform.eulerAngles.z
        );

        RotateClientRpc(newTargetRotation);

        Debug.Log($"Rotated by client {senderClientId}");
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void RotateClientRpc(Quaternion newTargetRotation)
    {
        targetRotation = newTargetRotation;
        isRotating = true;
        hasRotated = true;

        if (rotateSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(rotateSound);
        }
    }

    private void Update()
    {
        if (!isRotating) return;

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotateSpeed
        );

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.5f)
        {
            transform.rotation = targetRotation;
            isRotating = false;
        }
    }

    public string GetInteractText()
    {
        if (hasRotated)
            return "";

        if (requiredKey != null && !requiredKey.IsCollected)
            return "Locked - Missing Key";

        return "Rotate [E]";
    }
}