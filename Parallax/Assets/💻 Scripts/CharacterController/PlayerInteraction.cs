using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerInteraction : NetworkBehaviour
{
    [Header("General Interaction")]
    [SerializeField] private Transform interactOrigin;
    [SerializeField] private float interactDistance = 1f;
    [SerializeField] private float interactRadius = 1f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private InteractionPromptUI promptUI;
    
    [Header("Input Action References")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference moveAction;

    private BoxInteraction boxInteraction;
    private IInteractable currentInteractable;

    private void Awake()
    {
        boxInteraction = GetComponent<BoxInteraction>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        if (promptUI == null)
            promptUI = FindFirstObjectByType<InteractionPromptUI>();
    }
    
    void OnEnable()
    {
        interactAction?.action.Enable();
        moveAction?.action.Enable();
    }

    void OnDisable()
    {
        interactAction?.action.Disable();
        moveAction?.action.Disable();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (interactOrigin != null)
        {
            Debug.DrawRay(interactOrigin.position, interactOrigin.forward * interactDistance, Color.red);

            Vector3 end = interactOrigin.position + interactOrigin.forward * interactDistance;
            Debug.DrawLine(interactOrigin.position + Vector3.up * interactRadius, end + Vector3.up * interactRadius, Color.yellow);
            Debug.DrawLine(interactOrigin.position - Vector3.up * interactRadius, end - Vector3.up * interactRadius, Color.yellow);
        }

        UpdateInteractionPrompt();
        HandleBoxInput();
        HandleInteraction();
    }

    // OLD VERSION
    // private void HandleBoxInput()
    // {
    //     if (boxInteraction == null) return;
    //
    //     Vector2 input = Vector2.zero;
    //
    //     if (Keyboard.current != null)
    //     {
    //         if (Keyboard.current.aKey.isPressed) input.x -= 1f;
    //         if (Keyboard.current.dKey.isPressed) input.x += 1f;
    //         if (Keyboard.current.wKey.isPressed) input.y += 1f;
    //         if (Keyboard.current.sKey.isPressed) input.y -= 1f;
    //     }
    //
    //     boxInteraction.SetMoveInput(input);
    // }
    
    private void HandleBoxInput()
    {
        if (boxInteraction == null) return;
        if (moveAction == null) return;

        Vector2 input = moveAction.action.ReadValue<Vector2>();
        boxInteraction.SetMoveInput(input);
    }

    private void UpdateInteractionPrompt()
    {
        if (promptUI == null)
            promptUI = FindFirstObjectByType<InteractionPromptUI>();

        currentInteractable = FindInteractable();

        if (currentInteractable != null)
        {
            promptUI?.Show(currentInteractable.GetInteractText());
            return;
        }

        if (boxInteraction != null &&
            (boxInteraction.HasNearbyBox || boxInteraction.HasHeldBox || boxInteraction.HasAttachedBox))
        {
            promptUI?.Show("Press [E] to interact");
            return;
        }

        promptUI?.Hide();
    }

    private IInteractable FindInteractable()
    {
        if (interactOrigin == null) return null;

        Ray ray = GetInteractionRay();
        Vector3 rayDirection = ray.direction;

        if (Physics.SphereCast(ray, interactRadius, out RaycastHit hit, interactDistance, interactLayer))
        {
            IInteractable interactable =
                hit.collider.GetComponent<IInteractable>() ??
                hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
                return interactable;
        }

        Collider[] nearby = Physics.OverlapSphere(interactOrigin.position, interactRadius, interactLayer);

        IInteractable closestInteractable = null;
        float bestScore = -999f;

        foreach (Collider col in nearby)
        {
            IInteractable interactable =
                col.GetComponent<IInteractable>() ??
                col.GetComponentInParent<IInteractable>();

            if (interactable == null) continue;

            Vector3 toTarget = (col.bounds.center - interactOrigin.position).normalized;
            float dot = Vector3.Dot(rayDirection, toTarget);

            if (dot > bestScore)
            {
                bestScore = dot;
                closestInteractable = interactable;
            }
        }

        return closestInteractable;
    }

    private Ray GetInteractionRay()
    {
        Vector3 direction = interactOrigin.forward;

        if (Camera.main != null)
            direction = Camera.main.transform.forward;

        direction.Normalize();

        Vector3 origin = interactOrigin.position + direction * 0.1f;
        return new Ray(origin, direction);
    }

    private void HandleInteraction()
    {
        if (Keyboard.current == null) return;

        // OLD VERSION
        // if (Keyboard.current.eKey.wasPressedThisFrame)
        if (interactAction.action.WasPressedThisFrame())
        {
            if (TryInteractWithWorldObject())
                return;

            if (boxInteraction != null &&
                (boxInteraction.HasHeldBox ||
                 boxInteraction.HasAttachedBox ||
                 boxInteraction.HasNearbyBox))
            {
                boxInteraction.Interact();
            }
        }
    }

    private bool TryInteractWithWorldObject()
    {
        IInteractable interactable = FindInteractable();

        if (interactable != null)
        {
            interactable.Interact(gameObject);
            return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (interactOrigin == null) return;

        Gizmos.color = Color.red;

        Vector3 start = interactOrigin.position;
        Vector3 end = interactOrigin.position + interactOrigin.forward * interactDistance;

        Gizmos.DrawWireSphere(start, interactRadius);
        Gizmos.DrawWireSphere(end, interactRadius);
        Gizmos.DrawLine(start, end);
    }
}