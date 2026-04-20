using UnityEngine;

public class BoxInteraction : MonoBehaviour, IInteractor
{
    [Header("Detection")]
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask boxLayer;

    [Header("Carry")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float moveToHoldSpeed = 12f;

    [Header("Drag")]
    [SerializeField] private float dragDistance = 1.2f;

    private BoxInteractable nearbyBox;
    private BoxInteractable heldBox;
    private BoxInteractable attachedBox;

    private Vector3 dragDirection;
    private Vector2 moveInput;

    private RoleController roleController;
    private Movement movement;

    public bool HasNearbyBox => nearbyBox != null;
    public bool HasHeldBox => heldBox != null;
    public bool HasAttachedBox => attachedBox != null;
    public bool IsDraggingLargeBox => attachedBox != null;

    public BoxInteractable HeldBox => heldBox;
    public BoxInteractable AttachedBox => attachedBox;
    public Vector3 DragDirection => dragDirection;
    public Vector2 MoveInput => moveInput;
    public float DragDistance => dragDistance;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        roleController = GetComponent<RoleController>();
    }

    private void Update()
    {
        FindNearbyBox();
    }

    private void LateUpdate()
    {
        HandleHeldBox();
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void Interact()
    {
        Debug.Log("BoxInteraction.Interact called");

        if (roleController == null)
        {
            Debug.LogWarning("RoleController missing.");
            return;
        }

        if (!roleController.IsHuman)
        {
            Debug.Log("Cannot interact with box - player is not in human form.");
            return;
        }

        if (heldBox != null)
        {
            DropHeldBox();
            return;
        }

        if (attachedBox != null)
        {
            DetachBox();
            return;
        }

        if (nearbyBox == null)
        {
            Debug.Log("No nearby box found.");
            return;
        }

        Debug.Log($"Trying to interact with {nearbyBox.name}");

        if (nearbyBox.CanLift)
        {
            LiftBox(nearbyBox);
            return;
        }

        if (nearbyBox.CanPushOrPull)
        {
            AttachBox(nearbyBox);
            return;
        }

        Debug.Log("Box found, but it cannot be lifted or pushed/pulled.");
    }

    private void HandleHeldBox()
    {
        if (heldBox == null) return;

        Vector3 targetPos = holdPoint != null
            ? holdPoint.position
            : heldBox.GetCarryPosition(transform);

        heldBox.transform.position = Vector3.Lerp(
            heldBox.transform.position,
            targetPos,
            Time.deltaTime * moveToHoldSpeed
        );

        heldBox.transform.rotation = Quaternion.Lerp(
            heldBox.transform.rotation,
            Quaternion.identity,
            Time.deltaTime * moveToHoldSpeed
        );
    }

    private void FindNearbyBox()
    {
        nearbyBox = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, boxLayer);
        float closestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            BoxInteractable box = hit.GetComponentInParent<BoxInteractable>();
            if (box == null) continue;

            float distance = Vector3.Distance(transform.position, box.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearbyBox = box;
            }
        }
    }

    private void LiftBox(BoxInteractable box)
    {
        if (box == null) return;

        heldBox = box;
        attachedBox = null;
        moveInput = Vector2.zero;

        box.SetHeld(true);

        Debug.Log($"Lifted box: {box.name}");
    }

    private void DropHeldBox()
    {
        if (heldBox == null) return;

        heldBox.SetHeld(false);
        Debug.Log($"Dropped box: {heldBox.name}");

        heldBox = null;
        moveInput = Vector2.zero;
    }

    private void AttachBox(BoxInteractable box)
    {
        if (box == null) return;

        attachedBox = box;
        heldBox = null;
        moveInput = Vector2.zero;

        Rigidbody rb = box.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints |= RigidbodyConstraints.FreezeRotationX;
            rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
        }

        dragDirection = transform.position - box.transform.position;
        dragDirection.y = 0f;

        if (dragDirection.sqrMagnitude < 0.01f)
            dragDirection = -transform.forward;
        else
            dragDirection.Normalize();

        if (movement != null)
            movement.SetBoxDragMode(true);

        Debug.Log($"Attached to box: {box.name}");
    }

    public void RotateDragDirection(float turnAmountDegrees)
    {
        if (attachedBox == null) return;

        dragDirection = Quaternion.Euler(0f, turnAmountDegrees, 0f) * dragDirection;
        dragDirection.Normalize();
    }

    private void DetachBox()
    {
        if (attachedBox == null) return;

        Rigidbody rb = attachedBox.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            rb.constraints &= ~RigidbodyConstraints.FreezeRotationX;
            rb.constraints &= ~RigidbodyConstraints.FreezeRotationZ;
        }

        Debug.Log($"Detached from box: {attachedBox.name}");
        attachedBox = null;
        moveInput = Vector2.zero;

        if (movement != null)
            movement.SetBoxDragMode(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);

        if (attachedBox != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, attachedBox.transform.position);
        }
    }
}