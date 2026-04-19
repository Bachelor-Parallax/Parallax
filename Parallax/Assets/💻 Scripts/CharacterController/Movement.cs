using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class Movement : NetworkBehaviour, IMovement, ISprint
{
    [Header("Movement")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Audio")]
    [SerializeField] private AudioClip stepSound;
    [SerializeField] private float stepInterval = 0.5f;

    [Header("Strafe Turning")]
    [SerializeField] private float strafeTurnSpeed = 10f;
    [SerializeField] private float strafeTurnAngle = 18f;

    private AudioSource audioSource;
    private float stepTimer;

    public float Gravity => gravity;
    public float JumpHeight => jumpHeight;

    public bool MovementLocked { get; set; }
    public float SpeedMultiplier { get; set; } = 1f;

    private CharacterController controller;
    private JumpAbility jumpAbility;
    private BoxInteraction boxInteraction;
    private PlayerInteraction playerInteraction;
    private Transform cameraTransform;

    private float verticalVelocity;
    private bool isSprinting;
    private float freeLookYaw;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        jumpAbility = GetComponent<JumpAbility>();
        boxInteraction = GetComponent<BoxInteraction>();
        playerInteraction = GetComponent<PlayerInteraction>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        CursorManager.Lock();
        TryAssignCamera();
        freeLookYaw = transform.eulerAngles.y;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (cameraTransform == null)
            TryAssignCamera();

        if (MovementLocked)
        {
            ApplyGravityOnly();
            return;
        }

        if (boxInteraction != null && boxInteraction.enabled && boxInteraction.IsDraggingLargeBox)
        {
            ApplyGravityOnly();
            return;
        }

        HandleSprintInput();

        Vector2 input = GetMovementInput();

        Move(input);
        Rotate(input);

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
        if (Keyboard.current == null) return;

        bool sprinting = Keyboard.current.leftShiftKey.isPressed;
        SetSprinting(sprinting);
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

        bool rightMouseHeld = Mouse.current != null && Mouse.current.rightButton.isPressed;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Quaternion targetRotation = transform.rotation;

        if (rightMouseHeld)
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

            // Kun lidt body turn når man straf’er
            if (Mathf.Abs(input.x) > 0.01f)
                sideAngle = input.x * strafeTurnAngle;

            targetRotation = Quaternion.Euler(0f, freeLookYaw + sideAngle, 0f);
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            strafeTurnSpeed * Time.deltaTime
        );
    }

#endregion
#region Movement
    public void Move(Vector2 input)
    {
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        bool rightMouseHeld = Mouse.current != null && Mouse.current.rightButton.isPressed;

        Vector3 forward;
        Vector3 right;

        if (rightMouseHeld && cameraTransform != null)
        {
            forward = cameraTransform.forward;
            right = cameraTransform.right;
        }
        else
        {
            forward = transform.forward;
            right = transform.right;
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

    Vector2 GetMovementInput()
    {
        if (Keyboard.current == null)
            return Vector2.zero;

        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) input.y += 1f;
        if (Keyboard.current.sKey.isPressed) input.y -= 1f;
        if (Keyboard.current.aKey.isPressed) input.x -= 1f;
        if (Keyboard.current.dKey.isPressed) input.x += 1f;

        input = Vector2.ClampMagnitude(input, 1f);
        return input;
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

        if (playerInteraction != null)
            playerInteraction.enabled = role == CharacterRole.Human;
    }
}