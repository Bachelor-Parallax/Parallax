using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerInteraction : NetworkBehaviour
{
    [Header("General Interaction")]
    [SerializeField] private Transform interactOrigin;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactLayer;

    private RoleController roleController;
    private BoxInteraction boxInteraction;

    private void Awake()
    {
        roleController = GetComponent<RoleController>();
        boxInteraction = GetComponent<BoxInteraction>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (interactOrigin != null)
        {
            Debug.DrawRay(
                interactOrigin.position,
                interactOrigin.forward * interactDistance,
                Color.red
            );
        }

        if (Keyboard.current == null) return;

        if (!Keyboard.current.eKey.wasPressedThisFrame) return;

        if (roleController.CurrentRole == CharacterRole.Human)
        {
            HandleHumanInteraction();
        }
        else if (roleController.CurrentRole == CharacterRole.Cat)
        {
            HandleCatInteraction();
        }
    }

    // =========================
    // HUMAN
    // =========================
    private void HandleHumanInteraction()
    {
        Debug.Log("Human interact pressed");

        if (TryHumanInteraction())
            return;

        // fallback til box system
        if (boxInteraction != null &&
            (boxInteraction.HasHeldBox || boxInteraction.HasAttachedBox || boxInteraction.HasNearbyBox))
        {
            Debug.Log("Falling back to box interaction");
            boxInteraction.Interact();
        }
    }

    private bool TryHumanInteraction()
    {
        if (!TryGetHit(out RaycastHit hit))
            return false;

        IHumanInteractable interactable =
            hit.collider.GetComponent<IHumanInteractable>() ??
            hit.collider.GetComponentInParent<IHumanInteractable>();

        if (interactable != null)
        {
            Debug.Log($"Human interacting with: {hit.collider.name}");
            interactable.Interact(gameObject);
            return true;
        }

        Debug.Log("No human interactable found.");
        return false;
    }

    // =========================
    // CAT
    // =========================
    private void HandleCatInteraction()
    {
        Debug.Log("Cat interact pressed");

        TryCatInteraction();
    }

    private bool TryCatInteraction()
    {
        if (!TryGetHit(out RaycastHit hit))
            return false;

        ICatInteractable interactable =
            hit.collider.GetComponent<ICatInteractable>() ??
            hit.collider.GetComponentInParent<ICatInteractable>();

        if (interactable != null)
        {
            Debug.Log($"Cat interacting with: {hit.collider.name}");
            interactable.Interact(gameObject);
            return true;
        }

        Debug.Log("No cat interactable found.");
        return false;
    }

    // =========================
    // SHARED RAYCAST
    // =========================
    private bool TryGetHit(out RaycastHit hit)
    {
        hit = default;

        if (interactOrigin == null)
        {
            Debug.LogWarning("Interact origin is missing.");
            return false;
        }

        Ray ray = new Ray(interactOrigin.position, interactOrigin.forward);

        // DEBUG (som du havde før)
        RaycastHit[] hits = Physics.RaycastAll(ray, interactDistance);
        Debug.Log($"Total hits (no mask): {hits.Length}");

        foreach (var h in hits)
        {
            Debug.Log($"Hit: {h.collider.name} | Layer: {LayerMask.LayerToName(h.collider.gameObject.layer)}");
        }

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            Debug.Log($"Masked hit: {hit.collider.name}");
            return true;
        }

        Debug.Log("Nothing hit with mask.");
        return false;
    }
}