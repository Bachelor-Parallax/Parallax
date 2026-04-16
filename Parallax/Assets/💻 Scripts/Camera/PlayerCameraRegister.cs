using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerCameraRegister : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        StartCoroutine(BindCameraAfterSpawn());
    }

    private IEnumerator BindCameraAfterSpawn()
    {
        yield return null;
        yield return null;

        var cam = FindAnyObjectByType<CinemachineCamera>();
        if (cam == null)
        {
            Debug.LogError("[PCR] No CinemachineCamera found in scene.");
            yield break;
        }

        Transform target = transform.Find("CameraTarget");
        if (target == null)
            target = transform;

        cam.Follow = target;
        cam.LookAt = target;

        Debug.Log($"[PCR] Bound camera to {target.name} on runtime player {name}", this);
    }
}