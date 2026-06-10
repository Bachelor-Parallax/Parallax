using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerJumpAbility : MonoBehaviour, IJump
{
    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Jump Feel")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    [Header("Variable Jump")]
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    private PlayerMovement movement;
    private CharacterController controller;
    private PlayerAudioEvents playerAudioEvents;

    public bool JumpHeld { get; private set; }

    private float coyoteCounter;
    private float jumpBufferCounter;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        controller = GetComponent<CharacterController>();
        playerAudioEvents = GetComponent<PlayerAudioEvents>();
    }

    private void Update()
    {
        if (!movement.IsOwner) return;

        UpdateCoyoteTime();
        UpdateJumpBuffer();

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            PerformJump();
        }
    }

    public void RequestJump()
    {
        Debug.Log("REQUEST JUMP");
        if (!movement.IsOwner) return;

        JumpHeld = true;
        jumpBufferCounter = jumpBufferTime;
    }

    public void ReleaseJump()
    {
        JumpHeld = false;

        if (!movement.IsOwner) return;

        if (movement.VerticalVelocity > 0f)
        {
            movement.SetVerticalVelocity(movement.VerticalVelocity * jumpCutMultiplier);
        }
    }

    private void UpdateCoyoteTime()
    {
        if (movement.IsGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter = Mathf.Max(coyoteCounter - Time.deltaTime, 0f);
    }

    private void UpdateJumpBuffer()
    {
        jumpBufferCounter = Mathf.Max(jumpBufferCounter - Time.deltaTime, 0f);
    }

    private void PerformJump()
    {
        Debug.Log("Jump performed");

        float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        movement.SetVerticalVelocity(jumpVelocity);

        coyoteCounter = 0f;
        jumpBufferCounter = 0f;

        playerAudioEvents?.Jump();
    }

    public void SetJumpHeight(float value)
    {
        jumpHeight = value;
    }

    public void SetGravity(float value)
    {
        gravity = value;
    }
}