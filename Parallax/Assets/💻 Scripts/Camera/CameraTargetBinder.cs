using Unity.Cinemachine;
using UnityEngine;

public class CameraTargetBinder : MonoBehaviour
{
    private CinemachineCamera cinemachineCamera;
    private bool targetAssigned;

    private void Awake()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
    }

    private void Update()
    {
        if (targetAssigned) return;

        GameObject player = GameObject.Find("BasePlayerCharacter(Clone)");
        if (player == null) return;

        cinemachineCamera.Follow = player.transform;
        cinemachineCamera.LookAt = player.transform;
        targetAssigned = true;

        Debug.Log("Camera target assigned to spawned player clone.");
    }
}