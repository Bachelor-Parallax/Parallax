using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class LargeBoxDraggable : MonoBehaviour, IInteractable
{
    [Header("Drag")]
    [SerializeField] private float dragDistance = 1.2f;
    [SerializeField] private float dragMoveSpeed = 4f;
    [SerializeField] private float dragTurnSpeed = 120f;
    [SerializeField] private float playerSnapSpeed = 10f;

    private Rigidbody rb;
    private Collider boxCollider;

    private Transform holder;
    private Movement holderMovement;
    private CharacterController holderController;

    private Collider[] ignoredColliders;

    private bool isDragging;
    private Vector3 dragDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<Collider>();

        rb.constraints |= RigidbodyConstraints.FreezeRotationX;
        rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
        rb.linearDamping = 4f;
    }

    private void FixedUpdate()
    {
        if (!isDragging || holder == null)
            return;

        HandleDragMovement();
        HandlePlayerSnap();
    }

    public bool CanInteract(GameObject interactor)
    {
        RoleController role = interactor.GetComponent<RoleController>();

        if (role == null || !role.IsHuman)
            return false;

        return !isDragging || interactor.transform == holder;
    }

    public void Interact(GameObject interactor)
    {
        if (isDragging)
        {
            if (interactor.transform == holder)
                Detach(interactor);
        }
        else
        {
            Attach(interactor);
        }
    }

    private void Attach(GameObject interactor)
    {
        holder = interactor.transform;
        holderMovement = interactor.GetComponent<Movement>();
        holderController = interactor.GetComponent<CharacterController>();

        dragDirection = holder.position - transform.position;
        dragDirection.y = 0f;

        if (dragDirection.sqrMagnitude < 0.01f)
            dragDirection = -holder.forward;
        else
            dragDirection.Normalize();

        isDragging = true;

        ignoredColliders = interactor.GetComponentsInChildren<Collider>();

        foreach (Collider col in ignoredColliders)
            Physics.IgnoreCollision(boxCollider, col, true);

        holderMovement?.SetBoxDragMode(true);

        Interactor playerInteractor = interactor.GetComponent<Interactor>();
        playerInteractor?.SetActiveInteractable(this);
    }

    private void Detach(GameObject interactor)
    {
        isDragging = false;

        if (ignoredColliders != null)
        {
            foreach (Collider col in ignoredColliders)
                Physics.IgnoreCollision(boxCollider, col, false);
        }

        holderMovement?.SetBoxDragMode(false);

        rb.linearVelocity = Vector3.zero;

        holder = null;
        holderMovement = null;
        holderController = null;

        Interactor playerInteractor = interactor.GetComponent<Interactor>();
        playerInteractor?.ClearActiveInteractable(this);
    }
    
    private void HandleDragMovement()
    {
        if (holderMovement == null || holder == null)
            return;

        // constantly update drag direction
        dragDirection = holder.position - transform.position;
        dragDirection.y = 0f;

        if (dragDirection.sqrMagnitude > 0.001f)
            dragDirection.Normalize();

        Vector2 input = holderMovement.CurrentMoveInput;

        Vector3 forward = -dragDirection;
        Vector3 right = Vector3.Cross(Vector3.up, forward);

        Vector3 moveDir = (forward * input.y + right * input.x).normalized;

        rb.linearVelocity = new Vector3(
            moveDir.x * dragMoveSpeed,
            rb.linearVelocity.y,
            moveDir.z * dragMoveSpeed
        );
    }

    private void HandlePlayerSnap()
    {
        if (holderController == null)
            return;
    
        Vector3 targetPlayerPos = transform.position + dragDirection * dragDistance;
        targetPlayerPos.y = holder.position.y;
    
        Vector3 snapDelta = targetPlayerPos - holder.position;
    
        //holderController.Move(snapDelta * 0.5f);
        Vector3 move = Vector3.ClampMagnitude(snapDelta, playerSnapSpeed * Time.fixedDeltaTime);
        holderController.Move(move);
    
        Vector3 lookDir = transform.position - holder.position;
        lookDir.y = 0f;
    
        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
    
            holder.rotation = Quaternion.Slerp(
                holder.rotation,
                targetRot,
                playerSnapSpeed * Time.fixedDeltaTime
            );
        }
    }

    public string GetInteractText()
    {
        return isDragging ? "Release box" : "Push / Pull box";
    }
}