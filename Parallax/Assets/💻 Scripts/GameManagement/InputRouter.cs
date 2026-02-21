using UnityEngine;
using UnityEngine.InputSystem;

public class InputRouter : MonoBehaviour
{
    private IMovement movement;
    private IJump jump;

    public void SetActive(GameObject playerRoot)
    {
        movement = playerRoot.GetComponent<IMovement>();
        jump     = playerRoot.GetComponent<IJump>();
    }

    public void OnMove(InputValue value)
        => movement?.Move(value.Get<Vector2>());

    public void OnJump(InputValue _)
        => jump?.Jump();
}