using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerCube : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float tipTorque = 15f;

    private Rigidbody rb;
    private Vector2 moveInput;
    
    private float movementX;
    private float movementY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        LockRotation();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void OnMove(InputValue movementValue){
        UnityEngine.Vector2 movementVector = movementValue.Get<UnityEngine.Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            ResetTilt();
    }

    void FixedUpdate()
    {
        Vector3 moveDir = new Vector3(movementX, 0f, movementY);

        rb.linearVelocity = new Vector3(
            moveDir.x * moveSpeed,
            rb.linearVelocity.y,
            moveDir.z * moveSpeed
        );

        bool shift = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;

        if (shift)
        {
            UnlockRotation();

            if (moveDir.sqrMagnitude > 0.01f)
            {
                Vector3 torqueDir = new Vector3(moveDir.z, 0f, -moveDir.x);
                rb.AddTorque(torqueDir * tipTorque, ForceMode.Acceleration);
            }
        }
        else
        {
            LockRotation();
        }
    }

    void ResetTilt()
    {
        // Stop spinning immediately
        rb.angularVelocity = Vector3.zero;

        float yaw = rb.rotation.eulerAngles.y;
        rb.rotation = Quaternion.Euler(0f, yaw, 0f);

        LockRotation();
    }

    void LockRotation() =>
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

    void UnlockRotation() =>
        rb.constraints = RigidbodyConstraints.None;
}
