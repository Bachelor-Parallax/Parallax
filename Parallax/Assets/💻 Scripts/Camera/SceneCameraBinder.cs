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

        if (cam == null)
            Debug.LogError("[SceneCameraBinder] No CinemachineCamera found.", this);
    }

    private void LateUpdate()
    {
        if (bound || cam == null) return;

        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient)
            return;

        var playerObject = NetworkManager.Singleton.LocalClient?.PlayerObject;
        if (playerObject == null)
            return;

        Transform target = playerObject.transform.Find("CameraTarget");
        if (target == null)
        {
            Debug.LogWarning("CameraTarget not found, using root");
            target = playerObject.transform;
        }

        // 🔥 DET HER ER FIXET
        cam.Target.TrackingTarget = target;

        bound = true;

        Debug.Log($"[SceneCameraBinder] Bound camera to {target.name} on {playerObject.name}", this);
    }
}