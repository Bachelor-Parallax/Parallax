using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCatVisionInput : NetworkBehaviour
{
    [SerializeField] private InputActionReference catVisionAction;

    private PlayerCatVision playerCatVision;
    private RoleController roleController;

    private void Awake()
    {
        playerCatVision = GetComponent<PlayerCatVision>();
        roleController = GetComponent<RoleController>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        if (catVisionAction == null || catVisionAction.action == null)
        {
            Debug.LogWarning($"{nameof(PlayerCatVisionInput)}: catVisionAction is missing.");
            return;
        }

        catVisionAction.action.performed += OnVisionStarted;
        catVisionAction.action.canceled += OnVisionCanceled;

        catVisionAction.action.Enable();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        if (catVisionAction == null || catVisionAction.action == null)
            return;

        catVisionAction.action.performed -= OnVisionStarted;
        catVisionAction.action.canceled -= OnVisionCanceled;

        catVisionAction.action.Disable();
    }

    private void OnVisionStarted(InputAction.CallbackContext ctx)
    {
        if (roleController == null || !roleController.IsCat)
            return;

        playerCatVision?.SetVisionState(true);
    }

    private void OnVisionCanceled(InputAction.CallbackContext ctx)
    {
        playerCatVision?.SetVisionState(false);
    }
}