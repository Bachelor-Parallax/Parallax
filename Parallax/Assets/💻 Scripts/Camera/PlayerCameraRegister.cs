using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerCameraRegister : NetworkBehaviour
{
    [SerializeField] private Transform cameraTarget;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        var cam = FindFirstObjectByType<CinemachineCamera>();
        if (cam == null)
        {
            Debug.LogError("No CinemachineCamera found in scene.");
            return;
        }

        Transform targetToUse = cameraTarget != null ? cameraTarget : transform;

        cam.Follow = targetToUse;
        cam.LookAt = targetToUse;
    }
}