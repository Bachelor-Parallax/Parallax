using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class CatVision : NetworkBehaviour
{
    [SerializeField] private string glowRootName = "CatViewGlow";
    [SerializeField] private Key visionKey = Key.Q;

    [Header("Movement")]
    [SerializeField] private float visionMoveMultiplier = 0.35f;
    [SerializeField] private float movementBlendSpeed = 6f;

    [Header("Camera")]
    [SerializeField] private float zoomInAmount = 1.0f;

    private RoleController roleController;
    private CatVisionRoot glowRoot;
    private Movement movement;
    private FollowCam followCam;

    private bool isVisionActive;

    private float currentMoveMultiplier = 1f;
    private float targetMoveMultiplier = 1f;

    private float baseCameraDistance;

    private void Awake()
    {
        roleController = GetComponent<RoleController>();
        movement = GetComponent<Movement>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        followCam = GetComponentInChildren<FollowCam>(true);

        if (followCam != null)
        {
            baseCameraDistance = followCam.GetBaseDistance();
            followCam.SetTargetDistance(baseCameraDistance);
        }

        TryFindGlowRoot();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!IsOwner)
            return;

        TryFindGlowRoot();

        if (followCam == null)
            followCam = GetComponentInChildren<FollowCam>(true);

        if (followCam != null)
        {
            baseCameraDistance = followCam.GetBaseDistance();
            followCam.SetTargetDistance(baseCameraDistance);
        }
    }

    private void SetVisionState(bool active)
    {
        isVisionActive = active;

        if (glowRoot != null)
            glowRoot.SetTargetVisible(active);

        targetMoveMultiplier = active ? visionMoveMultiplier : 1f;

        if (followCam != null)
        {
            float targetDistance = active
                ? baseCameraDistance - zoomInAmount
                : baseCameraDistance;

            followCam.SetTargetDistance(targetDistance);
        }
    }

    private void TryFindGlowRoot()
    {
        GameObject rootObj = GameObject.Find(glowRootName);

        if (rootObj == null)
        {
            Debug.LogWarning($"CatVision: Could not find object named '{glowRootName}'");
            glowRoot = null;
            return;
        }

        glowRoot = rootObj.GetComponent<CatVisionRoot>();

        if (glowRoot == null)
        {
            Debug.LogWarning($"CatVision: '{glowRootName}' has no CatVisionRoot component.");
            return;
        }

        glowRoot.SetTargetVisible(false);
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if (roleController == null)
            return;

        bool keyHeld = Keyboard.current != null && Keyboard.current[visionKey].isPressed;

        if (!roleController.IsCat)
        {
            if (isVisionActive)
                SetVisionState(false);
        }
        else
        {
            if (keyHeld != isVisionActive)
                SetVisionState(keyHeld);
        }

        currentMoveMultiplier = Mathf.Lerp(
            currentMoveMultiplier,
            targetMoveMultiplier,
            movementBlendSpeed * Time.deltaTime
        );

        if (movement != null)
            movement.SpeedMultiplier = currentMoveMultiplier;
    }
}