using UnityEngine;
using UnityEngine.InputSystem;

public class BoxInteraction : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask Box;

    [Header("Carry")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float moveToHoldSpeed = 12f;

    [Header("Pull/Push")]
    [SerializeField] private float attachDistance = 1.2f;

    private BoxInteractable nearbyBox;
    private BoxInteractable heldBox;
    private BoxInteractable attachedBox;
    public bool IsDraggingLargeBox => attachedBox != null;

    private RoleController roleController;

    [SerializeField] private float dragDistance = 1.2f;
    [SerializeField] private float dragSpeed = 2.5f;
    [SerializeField] private float dragTurnSpeed = 40f;
    [SerializeField] private float snapSpeed = 10f;

    private Vector3 dragDirection;
    private TemporaryMovement movement;
    private CharacterController playerController;

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
        Debug.Log("Interact called");
        if (roleController == null)
        {
            Debug.LogWarning("RoleController missing");
            return;
        }
        if (!roleController.CanMoveBoxes)
        {
            Debug.Log("Cannot interact - not in human form");
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

        if (nearbyBox != null)
        {
            Debug.Log($"Trying to interact with {nearbyBox.name}");

            if (nearbyBox.CanLift)
            {
                LiftBox(nearbyBox);
            }
            else if (nearbyBox.CanPushOrPull)
            {
                AttachBox(nearbyBox);
            }
        }
        else
        {
            Debug.Log("No nearby box found");
        }
    }

    private void LateUpdate()
    {
        if (heldBox != null)
        {
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

        if (attachedBox != null)
        {
            HandleAttachedBoxMovement();
        }
    }

    private void HandleAttachedBoxMovement()
    {
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

        // Slow turning around Y axis
        if (Mathf.Abs(turnInput) > 0.01f)
        {
            float angle = turnInput * dragTurnSpeed * Time.deltaTime;
            dragDirection = Quaternion.Euler(0f, angle, 0f) * dragDirection;
            dragDirection.Normalize();
        }

        // Move box forward/backward based on player's relation to it
        Vector3 moveDir = -dragDirection * forwardInput;
        rb.linearVelocity = new Vector3(moveDir.x * dragSpeed, rb.linearVelocity.y, moveDir.z * dragSpeed);

        // Keep player snapped to the box
        Vector3 targetPlayerPos = attachedBox.transform.position + dragDirection * dragDistance;
        targetPlayerPos.y = transform.position.y;
        transform.position = Vector3.Lerp(transform.position, targetPlayerPos, Time.deltaTime * snapSpeed);

        // Player faces the box
        Vector3 lookDir = attachedBox.transform.position - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * snapSpeed);
        }
    }

    private void FindNearbyBox()
    {
        nearbyBox = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, Box);

        float closest = float.MaxValue;

        foreach (var hit in hits)
        {
            BoxInteractable box = hit.GetComponentInParent<BoxInteractable>();
            if (box == null) continue;

            float dist = Vector3.Distance(transform.position, box.transform.position);
            if (dist < closest)
            {
                closest = dist;
                nearbyBox = box;
            }
        }
    }

    private void LiftBox(BoxInteractable box)
    {
        heldBox = box;
        attachedBox = null;
        box.SetHeld(true);
    }

    private void DropHeldBox()
    {
        heldBox.SetHeld(false);
        heldBox = null;
    }

    private void AttachBox(BoxInteractable box)
    {
        attachedBox = box;
        heldBox = null;

        Rigidbody rb = box.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints |= RigidbodyConstraints.FreezeRotationX;
            rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
        }

        dragDirection = (transform.position - box.transform.position);
        dragDirection.y = 0f;
        dragDirection = dragDirection.normalized;

        if (dragDirection.sqrMagnitude < 0.01f)
            dragDirection = -transform.forward;

        Vector3 targetPlayerPos = box.transform.position + dragDirection * dragDistance;
        targetPlayerPos.y = transform.position.y;
        transform.position = targetPlayerPos;

        transform.rotation = Quaternion.LookRotation(-dragDirection);
    }

    private void DetachBox()
    {
        attachedBox = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}