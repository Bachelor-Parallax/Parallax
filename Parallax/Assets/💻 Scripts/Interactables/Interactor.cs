using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class Interactor : NetworkBehaviour
{
    [Header("Interaction")]
    [SerializeField] private Transform interactOrigin;
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private float interactRadius = 0.75f;
    [SerializeField] private LayerMask interactLayer;

    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;

    private IInteractable currentInteractable;
    private IInteractable activeInteractable;

    private InteractionPromptUI promptUI;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        interactAction?.action.Enable();

        promptUI = FindFirstObjectByType<InteractionPromptUI>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        currentInteractable = FindInteractable();

        UpdatePrompt();
        HandleInteraction();
    }

    private void UpdatePrompt()
    {
        IInteractable target = activeInteractable ?? currentInteractable;

        if (target != null && target.CanInteract(gameObject))
            promptUI?.Show(target.GetInteractText());
        else
            promptUI?.Hide();
    }

    private void HandleInteraction()
    {
        if (interactAction == null) return;

        if (interactAction.action.WasPressedThisFrame())
        {
            IInteractable target = activeInteractable ?? currentInteractable;

            if (target != null && target.CanInteract(gameObject))
            {
                target.Interact(gameObject);
            }
        }
    }

    private IInteractable FindInteractable()
    {
        if (interactOrigin == null) return null;

        Ray ray = new Ray(interactOrigin.position, interactOrigin.forward);

        if (Physics.SphereCast(ray, interactRadius, out RaycastHit hit, interactDistance, interactLayer))
        {
            return hit.collider.GetComponent<IInteractable>()
                   ?? hit.collider.GetComponentInParent<IInteractable>();
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

    public void SetActiveInteractable(IInteractable interactable)
    {
        activeInteractable = interactable;
    }

    public void ClearActiveInteractable(IInteractable interactable)
    {
        if (activeInteractable == interactable)
            activeInteractable = null;
    }
}