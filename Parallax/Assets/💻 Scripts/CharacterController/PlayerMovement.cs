using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour, IMovement, ISprint
{
    [Header("Movement")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController controller;
    private float verticalVelocity;
    private bool isSprinting;

    public float SpeedMultiplier { get; set; } = 1f;
    public bool MovementLocked { get; set; }

    public float VerticalVelocity => verticalVelocity;

    public bool IsGrounded => controller != null && controller.isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void Move(Vector2 input, Vector3 forward, Vector3 right)
    {
        if (MovementLocked) return;

        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Small negative value to keep the player grounded
        }

        verticalVelocity += gravity * Time.deltaTime;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 direction = (forward * input.y + right * input.x).normalized;
        float currentSpeed = isSprinting ? sprintSpeed : baseSpeed;

        Vector3 move = direction * (currentSpeed * SpeedMultiplier);
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    public void Teleport(Vector3 position)
    {
        if (controller != null)
            controller.enabled = false;

        transform.position = position;

        if (controller != null)
            controller.enabled = true;
    }

    public void SetSprinting(bool sprinting)
    {
        isSprinting = sprinting;
    }

    public void SetVerticalVelocity(float velocity)
    {
        verticalVelocity = velocity;
    }

    public void ResetVerticalVelocity()
    {
        verticalVelocity = 0f;
    }

    public void SetMovementLocked(bool locked)
    {
        MovementLocked = locked;
    }

    public void SetSprintSpeed(float newSprintSpeed)
    {
        sprintSpeed = newSprintSpeed;
    }

    public void SetGravity(float newGravity)
    {
        gravity = newGravity;
    }

    public void SetMoveSpeed(float newMoveSpeed)
    {
        baseSpeed = newMoveSpeed;
    }
}