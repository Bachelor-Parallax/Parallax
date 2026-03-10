using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class BaseController : NetworkBehaviour, IMovement
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

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        var cam = GetComponentInChildren<Camera>(true);

        if (cam == null)
        {
            Debug.LogError("Camera not found in player prefab!");
            return;
        }

        if (!IsOwner)
        {
            cam.gameObject.SetActive(false);
            enabled = false; // meget vigtigt
            return;
        }

        cam.gameObject.SetActive(true);

        followCam = cam.GetComponent<FollowCam>();
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

        HandleGravity();
        HandleMovement();
    }

    private void HandleMovement()
    {
        float camYaw = followCam != null ? followCam.Yaw : transform.eulerAngles.y;
        Quaternion yawRot = Quaternion.Euler(0f, camYaw, 0f);

        Vector3 camForward = yawRot * Vector3.forward;
        Vector3 camRight = yawRot * Vector3.right;

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