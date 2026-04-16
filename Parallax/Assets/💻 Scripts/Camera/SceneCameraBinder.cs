using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;

public class SceneCameraBinder : MonoBehaviour
{
    private CinemachineCamera cam;
    private bool bound;

    private void Awake()
    {
        cam = GetComponent<CinemachineCamera>();
    }

    private void LateUpdate()
    {
        if (bound) return;

        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient)
            return;

        var playerObject = NetworkManager.Singleton.LocalClient?.PlayerObject;
        if (playerObject == null)
            return;

        Transform target = playerObject.transform.Find("CameraTarget");
        if (target == null)
            target = playerObject.transform;

        cam.Follow = target;
        cam.LookAt = target;
        bound = true;

        Debug.Log($"[SceneCameraBinder] Bound camera to {target.name} on {playerObject.name}", this);
    }
}