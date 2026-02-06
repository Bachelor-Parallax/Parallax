using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerSphere : MonoBehaviour
{

    public float speed = 0f;
    
    private Rigidbody rb;
    private float movementX;
    private float movementY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnMove(InputValue movementValue)
    {
        UnityEngine.Vector2 movementVector = movementValue.Get<UnityEngine.Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void FixedUpdate()
    {
        UnityEngine.Vector3 movement = new UnityEngine.Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }


}
