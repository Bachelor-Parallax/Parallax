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
    [SerializeField] private float dragMoveSpeed = 4f;
    [SerializeField] private float dragTurnSpeed = 120f;
    [SerializeField] private float dragSnapSpeed = 10f;
    
    private Rigidbody _rb;

    private Transform _holder;
    private static bool _isHeld;
    private static bool _isDragging;

    private Vector3 _dragDirection;

    private Movement _holderMovement;
    
    private Collider _boxCollider;
    private Collider[] _playerColliders;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<Collider>();

        if (boxSize == BoxSize.Large)
        {
            _rb.constraints |= RigidbodyConstraints.FreezeRotationX;
            _rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
            _rb.linearDamping = 4f;
        }
    }

    public bool CanInteract(GameObject interactor)
    {
        RoleController role = interactor.GetComponent<RoleController>();
        return role != null && role.IsHuman;
    }

    public void Interact(GameObject interactor)
    {
        // if (_isDragging && interactor.transform == _holder)
        // {
        //     Detach();
        //     return;
        // }
        
        if (_isHeld)
        {
            Drop();
            return;
        }

        if (_isDragging)
        {
            Detach();
            return;
        }

        if (boxSize == BoxSize.Small && !_isHeld && !_isDragging)
        {
            Pickup(interactor);
            return;
        }

        if (boxSize == BoxSize.Large && !_isDragging && !_isHeld)
        {
            Attach(interactor);
        }
    }

    void Pickup(GameObject player)
    {
        _holder = player.transform;
        _holderMovement = player.GetComponent<Movement>();

        _isHeld = true;
        
        _playerColliders = player.GetComponentsInChildren<Collider>();

        foreach (Collider col in _playerColliders)
            Physics.IgnoreCollision(_boxCollider, col, true);

        _rb.isKinematic = true;
        _rb.useGravity = false;
    }

    void Drop()
    {
        _isHeld = false;

        if (_playerColliders != null)
        {
            foreach (Collider col in _playerColliders)
                Physics.IgnoreCollision(_boxCollider, col, false);
        }
        
        _holder = null;

        _rb.isKinematic = false;
        _rb.useGravity = true;
    }

    void Attach(GameObject player)
    {
        _holder = player.transform;
        _holderMovement = player.GetComponent<Movement>();

        _isDragging = true;
        
        _playerColliders = player.GetComponentsInChildren<Collider>();

        foreach (Collider col in _playerColliders)
            Physics.IgnoreCollision(_boxCollider, col, true);

        _dragDirection = _holder.position - transform.position;
        _dragDirection.y = 0f;

        if (_dragDirection.sqrMagnitude < 0.01f)
            _dragDirection = -_holder.forward;
        else
            _dragDirection.Normalize();

        if (_holderMovement != null)
            _holderMovement.SetBoxDragMode(true);
    }

    void Detach()
    {
        _isDragging = false;
        
        if (_playerColliders != null)
        {
            foreach (Collider col in _playerColliders)
                Physics.IgnoreCollision(_boxCollider, col, false);
        }

        if (_holderMovement != null)
            _holderMovement.SetBoxDragMode(false);

        _rb.linearVelocity = Vector3.zero;
        
        _holder = null;
    }

    private void LateUpdate()
    {
        if (_holder == null) return;

        if (_isHeld)
        {
            HandleCarry();
        }

        if (_isDragging)
        {
            HandleDragging();
        }
    }

    void FixedUpdate()
    {
        if (!_isDragging || _holder == null || _holderMovement == null)
            return;

        Vector2 input = _holderMovement.CurrentMoveInput;

        float forwardInput = input.y;
        float turnInput = input.x;

        // Rotate drag direction
        if (Mathf.Abs(turnInput) > 0.01f)
        {
            float angle = turnInput * dragTurnSpeed * Time.fixedDeltaTime;
            _dragDirection = Quaternion.Euler(0f, angle, 0f) * _dragDirection;
            _dragDirection.Normalize();
        }

        // Move box relative to drag direction
        Vector3 move = -_dragDirection * (forwardInput * dragMoveSpeed * Time.fixedDeltaTime);

        Vector3 targetPos = _rb.position + move;

        _rb.MovePosition(new Vector3(targetPos.x, _rb.position.y, targetPos.z));
    }
    
    void HandleCarry()
    {
        // THIS ALSO WORKS
        // Vector3 targetPos =
        //     _holder.position +
        //     _holder.forward * holdOffset.z +
        //     Vector3.up * holdOffset.y +
        //     _holder.right * holdOffset.x;
        //
        // transform.SetPositionAndRotation(
        //     Vector3.Lerp(transform.position, targetPos, Time.deltaTime * carryMoveSpeed),
        //     Quaternion.Lerp(transform.rotation, _holder.rotation, Time.deltaTime * carryMoveSpeed)
        // );

        // THIS WORKS
        float step = carryMoveSpeed * Time.deltaTime;
        Transform holdPoint = _holder.GetComponentInChildren<MouthCarryPoint>().transform;
        transform.position = Vector3.MoveTowards(transform.position, holdPoint.position + holdOffset, step);
    }
    
    void HandleDragging()
    {
        CharacterController controller = _holder.GetComponent<CharacterController>();

        if (controller != null)
        {
            Vector3 targetPlayerPos = transform.position + _dragDirection * dragDistance;
            targetPlayerPos.y = _holder.position.y;

            Vector3 snapDelta = targetPlayerPos - _holder.position;

            controller.Move(snapDelta * Mathf.Clamp01(Time.deltaTime * dragSnapSpeed));
        }

        Vector3 lookDir = transform.position - _holder.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);

            _holder.rotation = Quaternion.Slerp(
                _holder.rotation,
                targetRot,
                dragSnapSpeed * Time.deltaTime
            );
        }
    }

    public string GetInteractText()
    {
        if (_isHeld)
            return "Drop box";

        if (_isDragging)
            return "Release box";

        if (boxSize == BoxSize.Small)
            return "Pick up box";

        return "Push / Pull box";
    }
}