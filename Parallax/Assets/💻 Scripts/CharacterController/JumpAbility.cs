using UnityEngine;
using UnityEngine.InputSystem;

public class JumpAbility : MonoBehaviour
{
    private Movement movement;
    private CharacterController controller;
    
    [SerializeField] private AudioClip[] jumpSounds;
    [SerializeField] private InputActionReference jumpAction;
    
    private AudioSource audioSource;
    
    void Awake()
    {
        movement = GetComponent<Movement>();
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        if (jumpAction != null)
            jumpAction.action.Enable();
    }

    void OnDisable()
    {
        if (jumpAction != null)
            jumpAction.action.Disable();
    }

    void Update()
    {
        if (!enabled) return;
        if (movement == null || controller == null) return;
        if (jumpAction == null) return;

        if (jumpAction.action.WasPressedThisFrame() && controller.isGrounded)
        {
            movement.SetVerticalVelocity(Mathf.Sqrt(movement.JumpHeight * -2f * movement.Gravity));

            if (jumpSounds.Length > 0)
            {
                int index = Random.Range(0, jumpSounds.Length);
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(jumpSounds[index]);
            }
        }
    }
}