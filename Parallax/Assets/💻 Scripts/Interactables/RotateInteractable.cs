using Unity.Netcode;
using UnityEngine;

public class RotateInteractable : NetworkBehaviour, IInteractable
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    [Header("Rotation")]
    [SerializeField] private RotationAxis rotationAxis;
    [SerializeField] private float rotateAmount = 90f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private bool rotateOnlyOnce = true;

    [Header("Interaction Text")]
    [SerializeField] private string interactText = "Rotate [E]";
    [SerializeField] private string lockedText = "Locked - Missing Key";

    [Header("Requirements")]
    [SerializeField] private KeyInteractable requiredKey;

    [Header("Audio")]
    [SerializeField] private AudioClip rotateSound;

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
        if (rotateOnlyOnce && hasRotated) return false;
        if (isRotating) return false;

        RoleController role = interactor.GetComponent<RoleController>();
        return role != null && role.IsHuman;
    }

    public void Interact(GameObject interactor)
    {
        if (rotateOnlyOnce && hasRotated) return;
        if (isRotating) return;

        NetworkObject netObj = interactor.GetComponent<NetworkObject>();
        if (netObj == null) return;

        RotateServerRpc(netObj.OwnerClientId);
    }

    [Rpc(SendTo.Server)]
    private void RotateServerRpc(ulong senderClientId)
    {
        if (rotateOnlyOnce && hasRotated) return;
        if (isRotating) return;

        if (requiredKey != null && !requiredKey.IsCollected)
        {
            Debug.Log("Rotation locked - missing key.");
            return;
        }

        if (rotateOnlyOnce)
            hasRotated = true;

        Quaternion newTargetRotation = transform.rotation * Quaternion.Euler(GetRotationVector());

        RotateClientRpc(newTargetRotation);

        Debug.Log($"Rotated by client {senderClientId}");
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void RotateClientRpc(Quaternion newTargetRotation)
    {
        targetRotation = newTargetRotation;
        isRotating = true;

        if (rotateOnlyOnce)
            hasRotated = true;

        if (rotateSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(rotateSound);
        }
    }

    private Vector3 GetRotationVector()
    {
        return rotationAxis switch
        {
            RotationAxis.X => new Vector3(rotateAmount, 0f, 0f),
            RotationAxis.Y => new Vector3(0f, rotateAmount, 0f),
            RotationAxis.Z => new Vector3(0f, 0f, rotateAmount),
            _ => Vector3.zero
        };
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
        if (rotateOnlyOnce && hasRotated)
            return "";

        if (requiredKey != null && !requiredKey.IsCollected)
            return lockedText;

        return interactText;
    }
}