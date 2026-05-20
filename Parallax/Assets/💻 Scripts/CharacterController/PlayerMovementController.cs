using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : NetworkBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference rightClickAction;

    private PlayerMovement movement;
    private PlayerRotation rotation;
    private PlayerJumpAbility jumpAbility;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool rightMouseHeld;

    private Transform cameraTransform;

    public Vector2 CurrentMoveInput => moveInput;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        rotation = GetComponent<PlayerRotation>();
        jumpAbility = GetComponent<PlayerJumpAbility>();
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log($"PlayerMovementController OnNetworkSpawn | IsOwner: {IsOwner} | OwnerClientId: {OwnerClientId}");
        if (!IsOwner) return;
        
        Cursor.visible = false;

        TryAssignCamera();

        moveAction.action.Enable();
        sprintAction.action.Enable();
        lookAction.action.Enable();

        moveAction.action.performed += OnMove;
        moveAction.action.canceled += OnMove;

        sprintAction.action.performed += OnSprint;
        sprintAction.action.canceled += OnSprint;

        lookAction.action.performed += OnLook;
        lookAction.action.canceled += OnLook;

        rightClickAction.action.Enable();
        rightClickAction.action.performed += OnRightClick;
        rightClickAction.action.canceled += OnRightClick;

        jumpAction.action.Enable();
        jumpAction.action.performed += OnJump;
        jumpAction.action.canceled += OnJumpRelease;
        Debug.Log("Jump action subscribed: " + jumpAction.action.name);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        moveAction.action.performed -= OnMove;
        moveAction.action.canceled -= OnMove;

        sprintAction.action.performed -= OnSprint;
        sprintAction.action.canceled -= OnSprint;

        lookAction.action.performed -= OnLook;
        lookAction.action.canceled -= OnLook;

        rightClickAction.action.performed -= OnRightClick;
        rightClickAction.action.canceled -= OnRightClick;

        jumpAction.action.performed -= OnJump;
        jumpAction.action.canceled -= OnJumpRelease;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (cameraTransform == null)
            TryAssignCamera();

        if (cameraTransform == null)
            return;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        movement.Move(moveInput, forward, right);
        rotation.Rotate(moveInput, rightMouseHeld);
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.ClampMagnitude(ctx.ReadValue<Vector2>(), 1f);
    }

    private void OnSprint(InputAction.CallbackContext ctx)
    {
        movement.SetSprinting(ctx.ReadValueAsButton());
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    private void OnRightClick(InputAction.CallbackContext ctx)
    {
        rightMouseHeld = ctx.ReadValueAsButton();
    }

    private void TryAssignCamera()
    {
        if (Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        Debug.Log("JUMP INPUT");
       jumpAbility.RequestJump();
    }

    private void OnJumpRelease(InputAction.CallbackContext ctx)
    {
        jumpAbility.ReleaseJump();
    }
}