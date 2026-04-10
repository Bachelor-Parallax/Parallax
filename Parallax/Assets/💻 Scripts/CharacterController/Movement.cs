using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class Movement : NetworkBehaviour, IMovement
{
    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public float Gravity => gravity;
    public float JumpHeight => jumpHeight;

    public bool MovementLocked { get; set; }

    public float SpeedMultiplier { get; set; } = 1f;

    private CharacterController controller;
    private JumpAbility jumpAbility;
    private BoxInteraction boxInteraction;

    private float verticalVelocity;
    private FollowCam followCam;
    [SerializeField] private float rotationSpeed = 10f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        jumpAbility = GetComponent<JumpAbility>();
        boxInteraction = GetComponent<BoxInteraction>();
    }

    public override void OnNetworkSpawn()
    {
        followCam = GetComponentInChildren<FollowCam>(true);

        if (followCam == null)
        {
            Debug.LogError("FollowCam not found in player prefab!");
            return;
        }

        if (!IsOwner)
        {
            followCam.gameObject.SetActive(false);
        }
        else
        {
            followCam.gameObject.SetActive(true);
            followCam.SetTarget(transform, true);
        }
    }

    void Update()
    {
        if (!IsOwner || followCam == null) return;

        if (MovementLocked)
        {
            // Keep gravity so the player stays grounded naturally
            if (controller.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            verticalVelocity += gravity * Time.deltaTime;
            controller.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
            return;
        }

        var interaction = GetComponent<BoxInteraction>();
        if (interaction != null && interaction.enabled && interaction.IsDraggingLargeBox)
            return;

        Vector2 input = GetMovementInput();

        Move(input);
        Rotate();
    }

    public void SetVerticalVelocity(float value)
    {
        verticalVelocity = value;
    }

    private void Rotate()
    {
        Vector3 camForwardFlat = followCam.transform.forward;
        camForwardFlat.y = 0;
        camForwardFlat.Normalize();

        if (camForwardFlat.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(camForwardFlat);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    public void Move(Vector2 input)
    {
        Vector3 camForward = followCam.transform.forward;
        Vector3 camRight = followCam.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 direction = (camForward * input.y + camRight * input.x).normalized;

        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = direction * (speed * SpeedMultiplier);
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    Vector2 GetMovementInput()
    {
        if (Keyboard.current == null)
            return Vector2.zero;

        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) input.y += 1;
        if (Keyboard.current.sKey.isPressed) input.y -= 1;
        if (Keyboard.current.aKey.isPressed) input.x -= 1;
        if (Keyboard.current.dKey.isPressed) input.x += 1;

        return input;
    }

    public void ResetVerticalVelocity()
    {
        verticalVelocity = 0f;
    }

    public void Teleport(Vector3 pos)
    {
        if (controller) controller.enabled = false;
        transform.position = pos;
        if (controller) controller.enabled = true;
    }

    public void ApplyRole(CharacterRole role)
    {
        switch (role)
        {
            case CharacterRole.Human:
                speed = 5f;
                gravity = -9.81f;
                jumpHeight = 1f;
                break;

            case CharacterRole.Cat:
                speed = 8f;
                gravity = -9.81f;
                jumpHeight = 1.5f;
                break;

            default:
                speed = 5f;
                gravity = -9.81f;
                jumpHeight = 1f;
                if (jumpAbility != null) jumpAbility.enabled = false;
                if (boxInteraction != null) boxInteraction.enabled = false;
                break;
        }
        if (jumpAbility != null)
        jumpAbility.enabled = role == CharacterRole.Cat;

        if (boxInteraction != null)
        boxInteraction.enabled = role == CharacterRole.Human;
    }

}