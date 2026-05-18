using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerFootstepAudio : NetworkBehaviour
{
    [Header("Footsteps")]
    [SerializeField] private float walkStepInterval = 0.6f;
    [SerializeField] private float sprintStepInterval = 0.3f;
    [SerializeField] private float sprintSpeed = 9f;

    private CharacterController controller;
    private PlayerAudioEvents playerAudioEvents;
    private PlayerMovement movement;

    private float stepTimer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerAudioEvents = GetComponent<PlayerAudioEvents>();
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleFootsteps();
    }

    private void HandleFootsteps()
    {
        if (!controller.isGrounded)
        {
            stepTimer = 0f;
            return;
        }

        float horizontalSpeed = new Vector3(
            controller.velocity.x,
            0f,
            controller.velocity.z
        ).magnitude;

        if (horizontalSpeed < 0.1f)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            playerAudioEvents?.Footstep();

            float normalizedSpeed = Mathf.Clamp01(horizontalSpeed / sprintSpeed);

            stepTimer = Mathf.Lerp(
                walkStepInterval,
                sprintStepInterval,
                normalizedSpeed
            );
        }
    }
}