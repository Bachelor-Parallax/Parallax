using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public enum CameraMode
{
    AutoFollow,
    FreeLook
}

public class Movement : NetworkBehaviour, IMovement, ISprint
{
    [Header("Movement")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Audio")]
    [SerializeField] private AudioClip stepSound;

    [Header("Strafe Turning")]
    [SerializeField] private float strafeTurnSpeed = 10f;
    [SerializeField] private float strafeTurnAngle = 18f;

    [Header("Box Drag")]
    [SerializeField] private float dragMoveSpeed = 2.5f;
    [SerializeField] private float dragTurnSpeed = 120f;
    [SerializeField] private float dragSnapSpeed = 10f;

    [Header("Camera")]
    [SerializeField] private CameraMode cameraMode = CameraMode.AutoFollow;
    
    [Header("Input Action References")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference cameraRotateAction;

    private AudioSource audioSource;
    private float stepTimer;

    public float Gravity => gravity;
    public float JumpHeight => jumpHeight;
    
    public bool MovementLocked { get; set; }
    public float SpeedMultiplier { get; set; } = 1f;
    
    public Vector2 CurrentMoveInput { get; private set; }

    private CharacterController controller;
    //private JumpAbility jumpAbility;
    private BoxInteraction boxInteraction;
    private Transform cameraTransform;

    private float verticalVelocity;
    private float freeLookYaw;
    
    private bool isSprinting;
    private Vector2 lookInput;
    private Vector2 moveInput;
    private bool isBoxDragMode;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        //jumpAbility = GetComponent<JumpAbility>();
        boxInteraction = GetComponent<BoxInteraction>();
        audioSource = GetComponent<AudioSource>();
    }
    
    #region Network Events
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        CursorManager.Lock();
        TryAssignCamera();
        freeLookYaw = transform.eulerAngles.y;

        moveAction.action.Enable();
        sprintAction.action.Enable();
        cameraRotateAction.action.Enable();

        moveAction.action.performed += OnMove;
        moveAction.action.canceled += OnMove;

        sprintAction.action.performed += OnSprint;
        sprintAction.action.canceled += OnSprint;

        cameraRotateAction.action.performed += OnCameraRotate;
        cameraRotateAction.action.canceled += OnCameraRotate;
    }
    
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        moveAction.action.performed -= OnMove;
        moveAction.action.canceled -= OnMove;

        sprintAction.action.performed -= OnSprint;
        sprintAction.action.canceled -= OnSprint;

        cameraRotateAction.action.performed -= OnCameraRotate;
        cameraRotateAction.action.canceled -= OnCameraRotate;
    }
    #endregion
    
    #region Event Handlers
    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.ClampMagnitude(ctx.ReadValue<Vector2>(), 1f);
    }

    private void OnSprint(InputAction.CallbackContext ctx)
    {
        isSprinting = ctx.ReadValueAsButton();
    }

    private void OnCameraRotate(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }
    #endregion

    void Update()
    {
        if (!IsOwner) return;

        if (cameraTransform == null)
            TryAssignCamera();

        HandleSprintInput();

        if (boxInteraction != null && boxInteraction.enabled && boxInteraction.IsDraggingLargeBox)
        {
            HandleBoxDragMovement();
            HandleStepSound();
            return;
        }

        if (MovementLocked)
        {
            ApplyGravityOnly();
            return;
        }

        CurrentMoveInput = moveInput;

        Move(moveInput);
        Rotate(moveInput);

        HandleStepSound();
    }

    #region Sound
    private void HandleStepSound()
    {
        if (audioSource == null || stepSound == null) return;

        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(stepSound);

                float speed = controller.velocity.magnitude;
                stepTimer = Mathf.Lerp(0.6f, 0.3f, speed / sprintSpeed);
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }
    #endregion

    private void HandleSprintInput()
    {
        if (sprintAction == null) return;

        SetSprinting(sprintAction.action.IsPressed());
    }

    public void SetSprinting(bool sprinting)
    {
        isSprinting = sprinting;
    }

    private void TryAssignCamera()
    {
        if (Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void ApplyGravityOnly()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
    }

    public void SetVerticalVelocity(float value)
    {
        verticalVelocity = value;
    }

    #region Rotation
    private void Rotate(Vector2 input)
    {
        if (cameraTransform == null) return;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Quaternion targetRotation = transform.rotation;

        // AUTO FOLLOW MODE
        if (cameraMode == CameraMode.AutoFollow)
        {
            if (input.y > 0.1f) // kun fremad
            {
                Vector3 moveDir = (camForward * input.y + camRight * input.x).normalized;
                targetRotation = Quaternion.LookRotation(moveDir);
                freeLookYaw = targetRotation.eulerAngles.y;
            }
        }
        // FREE LOOK MODE
        else
        {
            if (lookInput.sqrMagnitude > 0.01f)
            {
                Vector3 targetDirection = Vector3.zero;

                if (input.sqrMagnitude > 0.01f)
                    targetDirection = (camForward * input.y + camRight * input.x).normalized;
                else
                    targetDirection = camForward;

                if (targetDirection.sqrMagnitude > 0.01f)
                {
                    targetRotation = Quaternion.LookRotation(targetDirection);
                    freeLookYaw = targetRotation.eulerAngles.y;
                }
            }
            else
            {
                float sideAngle = 0f;

                if (Mathf.Abs(input.x) > 0.01f)
                    sideAngle = input.x * strafeTurnAngle;

                targetRotation = Quaternion.Euler(0f, freeLookYaw + sideAngle, 0f);
            }
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            strafeTurnSpeed * Time.deltaTime
        );
    }

    public void SetCameraMode(CameraMode mode)
    {
        cameraMode = mode;

        Debug.Log("Camera mode set to: " + mode);

        if (cameraTransform != null)
            freeLookYaw = transform.eulerAngles.y;
    }
    #endregion

    #region Movement
    public void Move(Vector2 input)
    {
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 forward;
        Vector3 right;

        if ((cameraMode == CameraMode.AutoFollow && cameraTransform != null) ||
            (lookInput.sqrMagnitude > 0.01f && cameraTransform != null))
        {
            forward = cameraTransform.forward;
            right = cameraTransform.right;
        }
        else
        {
            Quaternion moveBasis = Quaternion.Euler(0f, freeLookYaw, 0f);
            forward = moveBasis * Vector3.forward;
            right = moveBasis * Vector3.right;
        }

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 direction = (forward * input.y + right * input.x).normalized;

        float currentSpeed = isSprinting ? sprintSpeed : baseSpeed;

        Vector3 move = direction * (currentSpeed * SpeedMultiplier);
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    private void HandleBoxDragMovement()
    {
        if (boxInteraction == null || !boxInteraction.IsDraggingLargeBox)
        {
            ApplyGravityOnly();
            return;
        }

        BoxInteractable box = boxInteraction.AttachedBox;
        if (box == null)
        {
            ApplyGravityOnly();
            return;
        }

        Rigidbody rb = box.GetComponent<Rigidbody>();
        if (rb == null)
        {
            ApplyGravityOnly();
            return;
        }

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        Vector2 dragInput = boxInteraction.MoveInput;
        float forwardInput = dragInput.y;
        float turnInput = dragInput.x;

        if (Mathf.Abs(turnInput) > 0.01f)
        {
            float angle = turnInput * dragTurnSpeed * Time.deltaTime;
            boxInteraction.RotateDragDirection(angle);
        }

        Vector3 dragDirection = boxInteraction.DragDirection;

        Vector3 moveDir = -dragDirection * forwardInput;
        rb.linearVelocity = new Vector3(
            moveDir.x * dragMoveSpeed,
            rb.linearVelocity.y,
            moveDir.z * dragMoveSpeed
        );

        Vector3 targetPlayerPos = box.transform.position + dragDirection * boxInteraction.DragDistance;
        targetPlayerPos.y = transform.position.y;

        Vector3 snapDelta = targetPlayerPos - transform.position;
        controller.Move(snapDelta * Mathf.Clamp01(Time.deltaTime * dragSnapSpeed));

        controller.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);

        Vector3 lookDir = box.transform.position - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                dragSnapSpeed * Time.deltaTime
            );
        }
    }

    public void SetBoxDragMode(bool enabled)
    {
        isBoxDragMode = enabled;
        MovementLocked = enabled;
    }

    // OLD VERSION
    // Vector2 GetMovementInput()
    // {
    //     if (Keyboard.current == null)
    //         return Vector2.zero;
    //
    //     Vector2 input = Vector2.zero;
    //
    //     if (Keyboard.current.wKey.isPressed) input.y += 1f;
    //     if (Keyboard.current.sKey.isPressed) input.y -= 1f;
    //     if (Keyboard.current.aKey.isPressed) input.x -= 1f;
    //     if (Keyboard.current.dKey.isPressed) input.x += 1f;
    //
    //     input = Vector2.ClampMagnitude(input, 1f);
    //     return input;
    // }
    
    Vector2 GetMovementInput()
    {
        if (!IsOwner) return Vector2.zero;
        if (moveAction == null)
            return Vector2.zero;

        return Vector2.ClampMagnitude(moveAction.action.ReadValue<Vector2>(), 1f);
    }
    #endregion

    public void ResetVerticalVelocity()
    {
        verticalVelocity = 0f;
    }

    public void Teleport(Vector3 pos)
    {
        if (controller) controller.enabled = false;
        transform.position = pos;
        if (controller) controller.enabled = true;
    }

    public void ApplyRole(CharacterRole role)
    {
        switch (role)
        {
            case CharacterRole.Human:
                baseSpeed = 5f;
                sprintSpeed = 8f;
                gravity = -9.81f;
                jumpHeight = 0.5f;
                break;

            case CharacterRole.Cat:
                baseSpeed = 8f;
                sprintSpeed = 12f;
                gravity = -9.81f;
                jumpHeight = 2f;
                break;
        }

        if (boxInteraction != null)
            boxInteraction.enabled = role == CharacterRole.Human;
    }
}