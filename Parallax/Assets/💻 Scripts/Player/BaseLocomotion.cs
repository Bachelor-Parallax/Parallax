using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BaseLocomotion : MonoBehaviour, ILocomotion
{
    [Header("Movement")]
    [SerializeField] [Range(0f, 20f)] public float moveSpeed = 5f;
    [SerializeField] [Range(0f, 36f)] private float rotationSpeed = 18f;

    [Header("Acceleration")]
    [SerializeField] [Range(0f, 10f)] private float acceleration = 5f;
    [SerializeField] [Range(0f, 10f)] private float deceleration = 5f;
    [SerializeField] [Range(0f, 10f)] private float airControl = 0.5f;

    [Header("Gravity")]
    [SerializeField] [Range(-50f, 0f)] private float gravity = -20f;
    [SerializeField] [Range(-10f, 0f)] private float groundStick = -2f;

    private CharacterController cc;
    private float yaw;
    private float verticalVelocity;
    private float speedMultiplier = 1f;

    private Vector2 moveInput;
    private Vector3 currentHorizontalVelocity;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        yaw = transform.eulerAngles.y;

        Cursor.visible = false;
    }

    void Update()
    {
        HandleGravity();
        HandleMovement();
    }

    private void HandleMovement()
    {
                Vector3 wishDirection =
            transform.forward * moveInput.y +
            transform.right * moveInput.x;

        wishDirection = Vector3.ClampMagnitude(wishDirection, 1f);

        float control = cc.isGrounded ? 1f : airControl;

        Vector3 targetVelocity =
            wishDirection * moveSpeed * speedMultiplier * control;

        float accelRate =
            (wishDirection.sqrMagnitude > 0.01f) ? acceleration : deceleration;

        currentHorizontalVelocity =
            Vector3.MoveTowards(
                currentHorizontalVelocity,
                targetVelocity,
                accelRate * Time.deltaTime
            );

        Vector3 motion =
            currentHorizontalVelocity +
            Vector3.up * verticalVelocity;

        cc.Move(motion * Time.deltaTime);
    }

    private void HandleGravity()
    {
        if (cc.isGrounded && verticalVelocity < 0f)
            verticalVelocity = groundStick;

        verticalVelocity += gravity * Time.deltaTime;
    }

    public void Move(Vector2 move)
    {
        moveInput = Vector2.ClampMagnitude(move, 1f);
    }

    public void Rotate(Vector2 look)
    {
        yaw += look.x * rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    public void AddVerticalImpulse(float impulse)
    {
        verticalVelocity = impulse;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = Mathf.Max(0f, multiplier);
    }
}
