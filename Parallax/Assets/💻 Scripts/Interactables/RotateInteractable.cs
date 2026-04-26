using Unity.Netcode;
using UnityEngine;

public class RotateInteractable : NetworkBehaviour, IInteractable, IActivationState
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

    [Header("Conditions")]
    [SerializeField] private IInteractCondition[] conditions;
    

    [Header("Audio")]
    [SerializeField] private AudioClip rotateSound;

    private AudioSource audioSource;
    private Quaternion targetRotation;
    private bool isRotating;
    private bool hasRotated;
    public bool IsActivated => hasRotated;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        targetRotation = transform.rotation;
        conditions = GetComponents<IInteractCondition>();
    }

    public bool CanInteract(GameObject interactor)
    {
        if (rotateOnlyOnce && hasRotated) return false;
        if (isRotating) return false;

        RoleController role = interactor.GetComponent<RoleController>();
        if (role == null || !role.IsHuman) return false;

        return ConditionsAreMet(interactor);
    }

    public void Interact(GameObject interactor)
    {
        if (!CanInteract(interactor)) return;

        NetworkObject netObj = interactor.GetComponent<NetworkObject>();
        if (netObj == null) return;

        RotateServerRpc(netObj.OwnerClientId);
    }

    private bool ConditionsAreMet(GameObject interactor)
    {
        foreach (MonoBehaviour condition in conditions)
        {
            if (condition is IInteractCondition interactCondition)
            {
                if (!interactCondition.IsMet(interactor))
                    return false;
            }
        }

        return true;
    }

    [Rpc(SendTo.Server)]
    private void RotateServerRpc(ulong senderClientId)
    {
        if (rotateOnlyOnce && hasRotated) return;
        if (isRotating) return;

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
        return interactText;
    }
}