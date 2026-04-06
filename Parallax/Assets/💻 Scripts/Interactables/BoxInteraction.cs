using UnityEngine;
using UnityEngine.InputSystem;

public class BoxInteraction : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask boxLayer;

    [Header("Carry")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float moveToHoldSpeed = 12f;

    [Header("Pull / Push")]
    [SerializeField] private float attachDistance = 1.2f;
    [SerializeField] private float dragDistance = 1.2f;
    [SerializeField] private float dragSpeed = 2.5f;
    [SerializeField] private float dragTurnSpeed = 40f;
    [SerializeField] private float snapSpeed = 10f;

    private BoxInteractable nearbyBox;
    private BoxInteractable heldBox;
    private BoxInteractable attachedBox;

    private Vector3 dragDirection;

    private RoleController roleController;
    private TemporaryMovement movement;
    private CharacterController playerController;

    public bool HasNearbyBox => nearbyBox != null;
    public bool HasHeldBox => heldBox != null;
    public bool HasAttachedBox => attachedBox != null;
    public bool IsDraggingLargeBox => attachedBox != null;

    private void Awake()
    {
        movement = GetComponent<TemporaryMovement>();
        playerController = GetComponent<CharacterController>();
        roleController = GetComponent<RoleController>();
    }

    private void Update()
    {
        FindNearbyBox();
    }

    public void Interact()
    {
        Debug.Log("BoxInteraction.Interact called");

        if (roleController == null)
        {
            Debug.LogWarning("RoleController missing.");
            return;
        }

        if (!roleController.CanMoveBoxes)
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

    private void LateUpdate()
    {
        HandleHeldBox();
        HandleAttachedBox();
    }

    private void HandleHeldBox()
    {
        if (heldBox == null) return;

        Vector3 targetPos = holdPoint != null
            ? holdPoint.position
            : heldBox.GetCarryPosition(transform);

        heldBox.transform.position = targetPos;

        heldBox.transform.rotation = Quaternion.Lerp(
            heldBox.transform.rotation,
            Quaternion.identity,
            Time.deltaTime * moveToHoldSpeed
        );
    }

    private void HandleAttachedBox()
    {
        if (attachedBox == null) return;

        Rigidbody rb = attachedBox.GetComponent<Rigidbody>();
        if (rb == null) return;

        float forwardInput = 0f;
        float turnInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) forwardInput += 1f;
            if (Keyboard.current.sKey.isPressed) forwardInput -= 1f;
            if (Keyboard.current.aKey.isPressed) turnInput -= 1f;
            if (Keyboard.current.dKey.isPressed) turnInput += 1f;
        }

        if (Mathf.Abs(turnInput) > 0.01f)
        {
            float angle = turnInput * dragTurnSpeed * Time.deltaTime;
            dragDirection = Quaternion.Euler(0f, angle, 0f) * dragDirection;
            dragDirection.Normalize();
        }

        Vector3 moveDir = -dragDirection * forwardInput;
        rb.linearVelocity = new Vector3(
            moveDir.x * dragSpeed,
            rb.linearVelocity.y,
            moveDir.z * dragSpeed
        );

        Vector3 targetPlayerPos = attachedBox.transform.position + dragDirection * dragDistance;
        targetPlayerPos.y = transform.position.y;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPlayerPos,
            Time.deltaTime * snapSpeed
        );

        Vector3 lookDir = attachedBox.transform.position - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * snapSpeed
            );
        }
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

        box.SetHeld(true);
        Debug.Log($"Lifted box: {box.name}");
    }

    private void DropHeldBox()
    {
        if (heldBox == null) return;

        heldBox.SetHeld(false);
        Debug.Log($"Dropped box: {heldBox.name}");

        heldBox = null;
    }

    private void AttachBox(BoxInteractable box)
    {
        if (box == null) return;

        attachedBox = box;
        heldBox = null;

        Rigidbody rb = box.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints |= RigidbodyConstraints.FreezeRotationX;
            rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
        }

        dragDirection = transform.position - box.transform.position;
        dragDirection.y = 0f;

        if (dragDirection.sqrMagnitude < 0.01f)
        {
            dragDirection = -transform.forward;
        }
        else
        {
            dragDirection.Normalize();
        }

        Vector3 targetPlayerPos = box.transform.position + dragDirection * dragDistance;
        targetPlayerPos.y = transform.position.y;
        transform.position = targetPlayerPos;

        transform.rotation = Quaternion.LookRotation(-dragDirection);

        Debug.Log($"Attached to box: {box.name}");
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