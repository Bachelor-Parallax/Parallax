using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class TemporaryMovement : NetworkBehaviour
{
    public float speed = 5f;

    private FollowCam followCam;

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

        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
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
}