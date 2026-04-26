using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Cinemachine;

public class CatVision : NetworkBehaviour
{
    private CatVisionTarget[] visionTargets;
    [SerializeField] private Key visionKey = Key.Q;

    [Header("Movement")]
    [SerializeField] private float visionMoveMultiplier = 0.35f;
    [SerializeField] private float movementBlendSpeed = 6f;

    [Header("Camera")]
    [SerializeField] private float zoomInAmount = 1.0f;
    [SerializeField] private float zoomSmoothSpeed = 5f;

    private float currentZoom = 0f;
    private float targetZoom = 0f;

    private RoleController roleController;
    private Movement movement;

    private CinemachineCamera cmCamera;
    private CinemachineOrbitalFollow orbitalFollow;

    private bool isVisionActive;

    private float currentMoveMultiplier = 1f;
    private float targetMoveMultiplier = 1f;

    private float baseTopRadius;
    private float baseCenterRadius;
    private float baseBottomRadius;

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
        if (!IsOwner) return;

        CacheCameraReferences();
        CacheVisionTargets();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!IsOwner) return;

        CacheCameraReferences();
        CacheVisionTargets();
    }

    private void CacheVisionTargets()
    {
        visionTargets = FindObjectsByType<CatVisionTarget>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (CatVisionTarget target in visionTargets)
        {
            target.SetVisible(false, true);
        }
    }

    private void CacheCameraReferences()
    {
        cmCamera = FindAnyObjectByType<CinemachineCamera>();

        if (cmCamera == null)
        {
            Debug.LogWarning("CatVision: No CinemachineCamera found.");
            orbitalFollow = null;
            return;
        }

        orbitalFollow = cmCamera.GetComponent<CinemachineOrbitalFollow>();

        if (orbitalFollow == null)
        {
            Debug.LogWarning("CatVision: No CinemachineOrbitalFollow found.");
            return;
        }

        baseTopRadius = orbitalFollow.Orbits.Top.Radius;
        baseCenterRadius = orbitalFollow.Orbits.Center.Radius;
        baseBottomRadius = orbitalFollow.Orbits.Bottom.Radius;
    }

    private void SetVisionState(bool active)
    {
        isVisionActive = active;

        if (visionTargets != null)
        {
            foreach (CatVisionTarget target in visionTargets)
            {
                if (target != null)
                    target.SetVisible(active);
            }
        }

        targetMoveMultiplier = active ? visionMoveMultiplier : 1f;
        targetZoom = active ? zoomInAmount : 0f;
    }

    private void UpdateCameraZoom()
    {
        if (orbitalFollow == null) return;

        currentZoom = Mathf.Lerp(
            currentZoom,
            targetZoom,
            zoomSmoothSpeed * Time.deltaTime
        );

        var orbits = orbitalFollow.Orbits;
        orbits.Top.Radius = Mathf.Max(0.5f, baseTopRadius - currentZoom);
        orbits.Center.Radius = Mathf.Max(0.5f, baseCenterRadius - currentZoom);
        orbits.Bottom.Radius = Mathf.Max(0.5f, baseBottomRadius - currentZoom);
        orbitalFollow.Orbits = orbits;
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (roleController == null) return;

        bool keyHeld = Keyboard.current != null && Keyboard.current[visionKey].isPressed;

        if (!roleController.IsCat)
        {
            if (isVisionActive)
                SetVisionState(false);
        }
        else
        {
            if (keyHeld != isVisionActive)
            {
                SetVisionState(keyHeld);
            }

            UpdateCameraZoom();
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