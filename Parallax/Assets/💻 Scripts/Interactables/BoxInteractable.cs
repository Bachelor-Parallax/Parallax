using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoxInteractable : MonoBehaviour, IInteractable
{
    public enum BoxSize { Small, Large }
    private enum BoxState { Idle, Held, Dragged }

    private static BoxInteractable activeBox;

    [Header("Type")]
    [SerializeField] private BoxSize boxSize;

    [Header("Carry")]
    [SerializeField] private Vector3 holdOffset;
    [SerializeField] private float carryMoveSpeed = 10f;

    [Header("Drag")]
    [SerializeField] private float dragDistance = 1.2f;
    [SerializeField] private float dragMoveSpeed = 4f;

    private Rigidbody rb;
    private Collider col;

    private Transform holder;
    private Movement movement;
    private CharacterController controller;
    private Transform holdPoint;

    private BoxState state = BoxState.Idle;
    private Vector3 dragDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public bool CanInteract(GameObject interactor)
    {
        RoleController role = interactor.GetComponent<RoleController>();
        if (role == null || !role.IsHuman)
            return false;

        return activeBox == null || activeBox == this;
    }

    public void Interact(GameObject interactor)
    {
        switch (state)
        {
            case BoxState.Idle:
                if (boxSize == BoxSize.Small)
                    Pickup(interactor);
                else
                    Attach(interactor);
                break;

            case BoxState.Held:
                Drop();
                break;

            case BoxState.Dragged:
                Detach();
                break;
        }
    }

    void Pickup(GameObject player)
    {
        activeBox = this;
        state = BoxState.Held;

        holder = player.transform;
        movement = player.GetComponent<Movement>();
        holdPoint = player.GetComponentInChildren<MouthCarryPoint>().transform;

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
    }

    void Drop()
    {
        state = BoxState.Idle;
        activeBox = null;

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;

        holder = null;
    }

    void Attach(GameObject player)
    {
        activeBox = this;
        state = BoxState.Dragged;

        holder = player.transform;
        movement = player.GetComponent<Movement>();
        controller = player.GetComponent<CharacterController>();

        dragDirection = (holder.position - transform.position).normalized;
        dragDirection.y = 0f;

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        movement.SetBoxDragMode(true);
    }

    void Detach()
    {
        state = BoxState.Idle;
        activeBox = null;

        movement.SetBoxDragMode(false);

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;

        holder = null;
    }

    void FixedUpdate()
    {
        if (holder == null) return;

        if (state == BoxState.Held)
            HandleCarry();

        if (state == BoxState.Dragged)
            HandleDrag();
    }

    void HandleCarry()
    {
        Vector3 target = holdPoint.position + holdOffset;

        Vector3 nextPos = Vector3.MoveTowards(
            rb.position,
            target,
            carryMoveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPos);
    }

    void HandleDrag()
    {
        Vector2 input = movement.CurrentMoveInput;

        Vector3 move = -dragDirection * input.y * dragMoveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        Vector3 targetPlayerPos = transform.position + dragDirection * dragDistance;
        Vector3 delta = targetPlayerPos - holder.position;
        delta.y = 0f;

        controller.Move(delta * 0.15f);
    }

    public string GetInteractText()
    {
        return state switch
        {
            BoxState.Held => "Drop box",
            BoxState.Dragged => "Release box",
            _ => boxSize == BoxSize.Small
                ? "Pick up box"
                : "Push / Pull box"
        };
    }
}