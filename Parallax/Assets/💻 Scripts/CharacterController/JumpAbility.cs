using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

using UnityEngine.SceneManagement;

public class JumpAbility : MonoBehaviour
{
    private Movement movement;
    private CharacterController controller;
    private PlayerAudioEvents playerAudioEvents;
    public bool jumpHeld { get; private set; }
    private float coyoteCounter;
    private float jumpBufferCounter;

    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;
    
    void Awake()
    {
        movement = GetComponent<Movement>();
        controller = GetComponent<CharacterController>();
        playerAudioEvents = GetComponent<PlayerAudioEvents>();
    }

    private void Start()
    {
        if (!movement.IsOwner) return;

        if (jumpAction != null)
        {
            jumpAction.action.Enable();
            jumpAction.action.performed += HandleJump;
            jumpAction.action.canceled += HandleJumpRelease;
        }
    }
    
    void Update()
    {
        if (!movement.IsOwner) return;

        // Coyote time
        if (controller.isGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter = Mathf.Max(coyoteCounter - Time.deltaTime, 0f);

        // Jump buffer
        jumpBufferCounter = Mathf.Max(jumpBufferCounter - Time.deltaTime, 0f);

        // Perform jump if both conditions are met
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            PerformJump();
        }
    }

    void OnDisable()
    {
        if (jumpAction != null)
        {
            jumpAction.action.performed -= HandleJump;
            jumpAction.action.canceled -= HandleJumpRelease;
            jumpAction.action.Disable();
        }
    }
    
    private void PerformJump()
    {
        movement.SetVerticalVelocity(
            Mathf.Sqrt(movement.JumpHeight * -2f * movement.Gravity)
        );

        coyoteCounter = 0f;
        jumpBufferCounter = 0f;

        if (playerAudioEvents != null)
        {
            playerAudioEvents?.Jump();
        }
    }
    
    private void HandleJump(InputAction.CallbackContext ctx)
    {
        if (!movement.IsOwner) return;
        
        jumpHeld = true;
        jumpBufferCounter = jumpBufferTime;
    }
    
    private void HandleJumpRelease(InputAction.CallbackContext ctx)
    {
        jumpHeld = false;
    }
}