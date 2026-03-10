using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class TemporaryMovement : NetworkBehaviour
{
    public float speed = 5f;

    void Start()
    {
        if (IsOwner)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
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
        }
        else
        {
            cam.gameObject.SetActive(true);

            // Assign target for FollowCam
            var follow = cam.GetComponent<FollowCam>();
            follow.target = transform;
        }
    }


    void Update()
    {
        if (!IsOwner) return;

        var cam = GetComponentInChildren<Camera>();
        if (cam == null) return;

        Vector2 input = GetMovementInput();

        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 direction = (camForward * input.y + camRight * input.x).normalized;

        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    
    Vector2 GetMovementInput()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            input.y += 1;

        if (Keyboard.current.sKey.isPressed)
            input.y -= 1;

        if (Keyboard.current.aKey.isPressed)
            input.x -= 1;

        if (Keyboard.current.dKey.isPressed)
            input.x += 1;

        return input;
    }

}