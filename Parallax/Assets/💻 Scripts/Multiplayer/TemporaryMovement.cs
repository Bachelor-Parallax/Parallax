using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class TemporaryMovement : NetworkBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    private CharacterController controller;
    private float verticalVelocity;

    private FollowCam followCam;

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
    
        Vector2 input = GetMovementInput();
    
        Vector3 camForward = followCam.transform.forward;
        Vector3 camRight = followCam.transform.right;
    
        camForward.y = 0;
        camRight.y = 0;
    
        camForward.Normalize();
        camRight.Normalize();
    
        Vector3 direction = (camForward * input.y + camRight * input.x).normalized;
        
        // Gravity
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;
        
        // direction.y = verticalVelocity;
        // controller.Move(direction * speed * Time.deltaTime);
        
        Vector3 move = direction * speed;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        if (direction.x != 0 || direction.z != 0)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
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
                speed = 5;
                gravity = -9.81f;
                jumpHeight = 1f;
                break;
            case CharacterRole.Cat:
                speed = 8;
                gravity = -9.81f;
                jumpHeight = 3f;
                break;
            default:
                speed = 5;
                gravity = -9.81f;
                jumpHeight = 1f;
                break;
        }
    }
}