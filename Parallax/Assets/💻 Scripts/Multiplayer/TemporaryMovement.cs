using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class TemporaryMovement : NetworkBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    private CharacterController controller;
    private float verticalVelocity;
    private FollowCam followCam;
    private bool canJump;

    [SerializeField] private float rotationSpeed = 10f; 

    void Awake()
    {
        controller = GetComponent<CharacterController>();
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

        var interaction = GetComponent<BoxInteraction>();
        if (interaction != null && interaction.IsDraggingLargeBox)
        {
            return;
        }
    
        Vector2 input = GetMovementInput();
    
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

        HandleJump();

        verticalVelocity += gravity * Time.deltaTime;
        
        Vector3 move = direction * speed;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

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

    void HandleJump()
    {
        if (!canJump) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            Debug.Log("Jump!");
        }
    }
    
    Vector2 GetMovementInput()
    {
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
                canJump = false; // hvis kun katten må hoppe
                break;

            case CharacterRole.Cat:
                speed = 8f;
                gravity = -9.81f;
                jumpHeight = 1.5f;
                canJump = true;
                break;

            default:
                speed = 5f;
                gravity = -9.81f;
                jumpHeight = 1f;
                canJump = false;
                break;
        }

        Debug.Log($"Role applied: {role}, canJump: {canJump}, jumpHeight: {jumpHeight}");
    }
}