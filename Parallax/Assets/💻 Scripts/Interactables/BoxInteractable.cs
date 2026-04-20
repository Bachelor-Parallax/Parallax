using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoxInteractable : MonoBehaviour, IInteractable
{
    public enum BoxSize
    {
        Small,
        Large
    }

    [Header("Box Type")]
    [SerializeField] private BoxSize boxSize = BoxSize.Small;

    [Header("Carry Settings")]
    [SerializeField] private Vector3 holdOffset = new Vector3(0f, 1.2f, 0.8f);
    [SerializeField] private float carryMoveSpeed = 12f;

    [Header("Drag Settings")]
    [SerializeField] private float dragDistance = 1.2f;

    private Rigidbody rb;

    private Transform holder;
    private bool isHeld;
    private bool isDragging;

    private Vector3 dragDirection;

    private Movement holderMovement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (boxSize == BoxSize.Large)
        {
            rb.constraints |= RigidbodyConstraints.FreezeRotationX;
            rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
            rb.linearDamping = 4f;
        }
    }

    public bool CanInteract(GameObject interactor)
    {
        RoleController role = interactor.GetComponent<RoleController>();
        return role != null && role.IsHuman;
    }

    public void Interact(GameObject interactor)
    {
        if (isHeld)
        {
            Drop();
            return;
        }

        if (isDragging)
        {
            Detach();
            return;
        }

        if (boxSize == BoxSize.Small)
        {
            Pickup(interactor);
            return;
        }

        if (boxSize == BoxSize.Large)
        {
            Attach(interactor);
        }
    }

    void Pickup(GameObject player)
    {
        holder = player.transform;
        holderMovement = player.GetComponent<Movement>();

        isHeld = true;

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Drop()
    {
        isHeld = false;

        holder = null;

        rb.isKinematic = false;
        rb.useGravity = true;
    }

    void Attach(GameObject player)
    {
        holder = player.transform;
        holderMovement = player.GetComponent<Movement>();

        isDragging = true;

        dragDirection = holder.position - transform.position;
        dragDirection.y = 0;

        if (dragDirection.sqrMagnitude < 0.01f)
            dragDirection = -holder.forward;
        else
            dragDirection.Normalize();

        if (holderMovement != null)
            holderMovement.SetBoxDragMode(true);
    }

    void Detach()
    {
        isDragging = false;

        if (holderMovement != null)
            holderMovement.SetBoxDragMode(false);

        holder = null;
    }

    void LateUpdate()
    {
        if (isHeld && holder != null)
        {
            Vector3 targetPos =
                holder.position +
                holder.forward * holdOffset.z +
                Vector3.up * holdOffset.y +
                holder.right * holdOffset.x;

            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * carryMoveSpeed
            );

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.identity,
                Time.deltaTime * carryMoveSpeed
            );
        }
    }

    void FixedUpdate()
    {
        if (!isDragging || holder == null || holderMovement == null)
            return;

        Vector2 moveInput = holderMovement.CurrentMoveInput;

        Vector3 move =
            holder.forward * moveInput.y +
            holder.right * moveInput.x;

        Vector3 targetPos =
            holder.position - dragDirection * dragDistance;

        Vector3 newPos = targetPos + move * Time.fixedDeltaTime * 4f;

        rb.MovePosition(new Vector3(newPos.x, rb.position.y, newPos.z));
    }

    public string GetInteractText()
    {
        if (isHeld)
            return "Drop box";

        if (isDragging)
            return "Release box";

        if (boxSize == BoxSize.Small)
            return "Pick up box";

        return "Push / Pull box";
    }
}