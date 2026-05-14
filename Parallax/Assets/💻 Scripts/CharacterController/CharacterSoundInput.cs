using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSoundInput : NetworkBehaviour
{
    [SerializeField] private InputActionReference soundAction;

    private PlayerAudioEvents playerAudioEvents;

    private void Awake()
    {
        playerAudioEvents = GetComponent<PlayerAudioEvents>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        if (soundAction == null || soundAction.action == null)
        {
            Debug.LogWarning("CharacterSoundInput: soundAction is missing.");
            return;
        }

        soundAction.action.performed += OnSound;
        soundAction.action.Enable();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        if (soundAction != null && soundAction.action != null)
        {
            soundAction.action.performed -= OnSound;
            soundAction.action.Disable();
        }
    }

    private void OnSound(InputAction.CallbackContext ctx)
    {
        Debug.Log("Sound input pressed");
        playerAudioEvents?.CharacterSound();
    }
}