using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private IMovement movement;
    private IJump jump;
    private ISprint sprint;

    void Awake()
    {
        movement = GetComponent<IMovement>();
        jump = GetComponent<IJump>();
        sprint = GetComponent<ISprint>();  
    }

    public void OnMove(InputValue value) => movement?.Move(value.Get<Vector2>());
    public void OnLook(InputValue value) => movement?.Rotate(value.Get<Vector2>());

    public void OnJump(InputValue _)
    {
        jump?.Jump();
    }

    public void OnSprint(InputValue value)
    {
        sprint?.SetSprinting(value.isPressed);
    }
}