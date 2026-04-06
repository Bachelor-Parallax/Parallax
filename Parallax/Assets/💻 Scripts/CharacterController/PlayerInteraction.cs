using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerInteraction : NetworkBehaviour
{
    [Header("General Interaction")]
    [SerializeField] private Transform interactOrigin;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactLayer;

    private BoxInteraction boxInteraction;

    private void Awake()
    {
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

        HandleInteraction();
    }

    private void HandleInteraction()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("Interact pressed");

            if (TryInteractWithWorldObject())
                return;

            if (boxInteraction != null &&
                (boxInteraction.HasHeldBox || boxInteraction.HasAttachedBox || boxInteraction.HasNearbyBox))
            {
                Debug.Log("Falling back to box interaction");
                boxInteraction.Interact();
            }
        }
    }

    private bool TryInteractWithWorldObject()
    {
        if (interactOrigin == null)
        {
            Debug.LogWarning("Interact origin is missing.");
            return false;
        }

        Debug.Log($"Interact origin position: {interactOrigin.position}");
        Debug.Log($"Interact origin forward: {interactOrigin.forward}");
        Debug.Log($"Interact distance: {interactDistance}");
        Debug.Log($"Interact layer mask value: {interactLayer.value}");

        Ray ray = new Ray(interactOrigin.position, interactOrigin.forward);

        RaycastHit[] hits = Physics.RaycastAll(ray, interactDistance);

        Debug.Log($"Total hits without layer mask: {hits.Length}");

        foreach (RaycastHit h in hits)
        {
            Debug.Log(
                $"Raw hit: {h.collider.name} | Layer: {LayerMask.LayerToName(h.collider.gameObject.layer)}"
            );
        }

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            Debug.Log($"Masked hit: {hit.collider.name}");
            Debug.Log($"Masked hit layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

            IInteractable interactable =
                hit.collider.GetComponent<IInteractable>() ??
                hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                Debug.Log($"Interacting with: {hit.collider.name}");
                interactable.Interact(gameObject);
                return true;
            }

            Debug.Log("Hit object has no IInteractable.");
        }
        else
        {
            Debug.Log("Nothing interactable hit with current layer mask.");
        }

        return false;
    }
}