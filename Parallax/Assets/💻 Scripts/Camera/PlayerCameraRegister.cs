using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerCameraRegister : NetworkBehaviour
{
    [SerializeField] private Transform cameraTarget;

    private static bool cameraAssigned = false;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        // Sikrer vi kun binder EN gang per klient
        if (cameraAssigned) return;

        var cam = FindFirstObjectByType<CinemachineCamera>();
        if (cam == null)
        {
            Debug.LogError("No CinemachineCamera found in scene.");
            return;
        }

        Transform targetToUse = cameraTarget != null ? cameraTarget : transform;

        cam.Follow = targetToUse;
        cam.LookAt = targetToUse;

        cameraAssigned = true;

        Debug.Log($"Camera bound to: {targetToUse.name} (Owner: {OwnerClientId})", this);
    }
}