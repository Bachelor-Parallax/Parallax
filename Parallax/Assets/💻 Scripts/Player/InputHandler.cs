using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private ILocomotion locomotion;
    private IJump jump;
    private ISprint sprint;

    void Awake()
    {
        locomotion = GetComponent<ILocomotion>();
        jump = GetComponent<IJump>();
        sprint = GetComponent<ISprint>();  
    }

    public void OnMove(InputValue value) => locomotion?.Move(value.Get<Vector2>());
    public void OnLook(InputValue value) => locomotion?.Rotate(value.Get<Vector2>());

    public void OnJump(InputValue _)
    {
        jump?.Jump();
    }

    public void OnSprint(InputValue value)
    {
        sprint?.SetSprinting(value.isPressed);
    }
}
