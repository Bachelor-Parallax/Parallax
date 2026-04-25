using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class JumpAbility : MonoBehaviour
{
    private Movement movement;
    private CharacterController controller;
    public bool jumpHeld { get; private set; }
    private float coyoteCounter;
    private float jumpBufferCounter;

    
    [SerializeField] private AudioClip[] jumpSounds;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;
    
    private AudioSource audioSource;
    
    void Awake()
    {
        movement = GetComponent<Movement>();
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
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

        if (jumpSounds.Length > 0)
        {
            int index = Random.Range(0, jumpSounds.Length);
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(jumpSounds[index]);
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