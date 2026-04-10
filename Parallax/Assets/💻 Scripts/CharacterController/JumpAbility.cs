using UnityEngine;
using UnityEngine.InputSystem;

public class JumpAbility : MonoBehaviour
{
    private Movement movement;
    private CharacterController controller;

    void Awake()
    {
        movement = GetComponent<Movement>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!enabled) return;
        if (Keyboard.current == null) return;
        if (movement == null || controller == null) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && controller.isGrounded)
        {
            movement.SetVerticalVelocity(Mathf.Sqrt(movement.JumpHeight * -2f * movement.Gravity));
        }
    }
}