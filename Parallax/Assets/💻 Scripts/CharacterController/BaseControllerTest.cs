using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class BaseControllerTest : NetworkBehaviour, IMovement
{
    [Header("Movement")]
    [SerializeField] [Range(0f, 20f)] private float moveSpeed = 5f;
    [SerializeField] [Range(0f, 36f)] private float rotationSpeed = 18f;

    [Header("Acceleration")]
    [SerializeField] [Range(0f, 10f)] private float acceleration = 5f;
    [SerializeField] [Range(0f, 10f)] private float deceleration = 5f;
    [SerializeField] [Range(0f, 10f)] private float airControl = 0.5f;

    [Header("Gravity")]
    [SerializeField] [Range(-50f, 0f)] private float gravity = -20f;
    [SerializeField] [Range(-10f, 0f)] private float groundStick = -2f;

    private CharacterController cc;
    private float verticalVelocity;
    private float speedMultiplier = 1f;

    private Vector2 moveInput;
    private Vector3 currentHorizontalVelocity;
    private FollowCam followCam;
    private Camera playerCam;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        playerCam = GetComponentInChildren<Camera>(true);

        if (playerCam == null)
        {
            Debug.LogError("Camera not found in player prefab!");
            return;
        }

        if (!IsOwner)
        {
            playerCam.gameObject.SetActive(false);
            enabled = false;
            return;
        }

        playerCam.gameObject.SetActive(true);

        followCam = playerCam.GetComponent<FollowCam>();
        if (followCam != null)
        {
            followCam.SetTarget(transform);
        }
        else
        {
            Debug.LogWarning("FollowCam not found on local player camera.");
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        moveInput = GetMovementInput();

        HandleGravity();
        HandleMovement();
    }

    private Vector2 GetMovementInput()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) input.y += 1;
        if (Keyboard.current.sKey.isPressed) input.y -= 1;
        if (Keyboard.current.aKey.isPressed) input.x -= 1;
        if (Keyboard.current.dKey.isPressed) input.x += 1;

        return Vector2.ClampMagnitude(input, 1f);
    }

    private void HandleMovement()
    {
        if (playerCam == null) return;

        Vector3 camForward = playerCam.transform.forward;
        Vector3 camRight = playerCam.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 wishDirection = camForward * moveInput.y + camRight * moveInput.x;
        wishDirection = Vector3.ClampMagnitude(wishDirection, 1f);

        if (wishDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(wishDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        float control = cc.isGrounded ? 1f : airControl;

        Vector3 targetVelocity = wishDirection * moveSpeed * speedMultiplier;
        targetVelocity *= control;

        float accelRate = wishDirection.sqrMagnitude > 0.01f ? acceleration : deceleration;

        currentHorizontalVelocity = Vector3.MoveTowards(
            currentHorizontalVelocity,
            targetVelocity,
            accelRate * Time.deltaTime
        );

        Vector3 motion = currentHorizontalVelocity;
        motion.y = verticalVelocity;

        cc.Move(motion * Time.deltaTime);
    }

    private void HandleGravity()
    {
        if (cc.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundStick;
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    public void Move(Vector2 move)
    {
        moveInput = Vector2.ClampMagnitude(move, 1f);
    }

    public void Rotate(Vector2 look)
    {
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