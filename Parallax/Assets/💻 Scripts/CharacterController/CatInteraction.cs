using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class CatInteraction : NetworkBehaviour
{
    [Header("Interaction")]
    [SerializeField] private Transform interactOrigin;
    [SerializeField] private float interactDistance = 1.2f;
    [SerializeField] private float interactRadius = 0.8f;
    [SerializeField] private LayerMask interactLayer;

    [SerializeField] private InteractionPromptUI promptUI;

    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;

    private IInteractable currentInteractable;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        interactAction?.action.Enable();

        if (promptUI == null)
            promptUI = FindFirstObjectByType<InteractionPromptUI>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        UpdateInteractionPrompt();
        HandleInteraction();
    }

    private void UpdateInteractionPrompt()
    {
        currentInteractable = FindInteractable();

        if (currentInteractable != null)
        {
            promptUI?.Show(currentInteractable.GetInteractText());
        }
        else
        {
            promptUI?.Hide();
        }
    }

    private void HandleInteraction()
    {
        if (interactAction == null) return;

        if (interactAction.action.WasPressedThisFrame())
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact(gameObject);
            }
        }
    }

    private IInteractable FindInteractable()
    {
        if (interactOrigin == null) return null;

        Ray ray = new Ray(interactOrigin.position, interactOrigin.forward);

        if (Physics.SphereCast(ray, interactRadius, out RaycastHit hit, interactDistance, interactLayer))
        {
            IInteractable interactable =
                hit.collider.GetComponent<IInteractable>() ??
                hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
                return interactable;
        }

        Collider[] nearby = Physics.OverlapSphere(interactOrigin.position, interactRadius, interactLayer);

        foreach (Collider col in nearby)
        {
            IInteractable interactable =
                col.GetComponent<IInteractable>() ??
                col.GetComponentInParent<IInteractable>();

            if (interactable != null)
                return interactable;
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        if (interactOrigin == null) return;

        Gizmos.color = Color.cyan;

        Vector3 start = interactOrigin.position;
        Vector3 end = interactOrigin.position + interactOrigin.forward * interactDistance;

        Gizmos.DrawWireSphere(start, interactRadius);
        Gizmos.DrawWireSphere(end, interactRadius);
        Gizmos.DrawLine(start, end);
    }
}