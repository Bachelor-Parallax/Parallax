using Unity.Netcode;
using UnityEngine;

public enum CameraMode
{
    AutoFollow,
    FreeLook
}

public class PlayerRotation : NetworkBehaviour
{
    [Header("Rotation")]
    [SerializeField] private CameraMode cameraMode = CameraMode.AutoFollow;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Strafe Turning")]
    [SerializeField] private float strafeTurnAngle = 18f;

    private Transform cameraTransform;
    private float freeLookYaw;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        TryAssignCamera();
        freeLookYaw = transform.eulerAngles.y;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (cameraTransform == null)
            TryAssignCamera();
    }

    public void Rotate(Vector2 moveInput, bool rightMouseHeld)
    {
        if (!IsOwner) return;
        if (cameraTransform == null) return;

        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Quaternion targetRotation = transform.rotation;

        if (rightMouseHeld)
        {
            targetRotation = Quaternion.LookRotation(camForward);
            freeLookYaw = targetRotation.eulerAngles.y;
        }
        else if (moveInput.y > 0.1f)
        {
            targetRotation = Quaternion.LookRotation(camForward);
            freeLookYaw = targetRotation.eulerAngles.y;
        }
        else
        {
            float sideAngle = 0f;

            if (Mathf.Abs(moveInput.x) > 0.01f)
                sideAngle = moveInput.x * strafeTurnAngle;

            targetRotation = Quaternion.Euler(0f, freeLookYaw + sideAngle, 0f);
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void SetCameraMode(CameraMode mode)
    {
        cameraMode = mode;

        if (cameraTransform != null)
            freeLookYaw = transform.eulerAngles.y;
    }

    private void TryAssignCamera()
    {
        if (Camera.main != null)
            cameraTransform = Camera.main.transform;
    }
}