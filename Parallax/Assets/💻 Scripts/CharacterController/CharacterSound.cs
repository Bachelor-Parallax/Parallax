using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class CharacterSound : NetworkBehaviour
{
    [SerializeField] private AudioClip[] catSounds;
    [SerializeField] private AudioClip[] humanSounds;
    [SerializeField] private InputActionReference soundAction;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (soundAction == null) return;

        soundAction.action.performed += OnSound;
        soundAction.action.Enable();
    }

    private void OnDisable()
    {
        if (soundAction == null) return;

        soundAction.action.performed -= OnSound;
        soundAction.action.Disable();
    }
    
    private void OnSound(InputAction.CallbackContext context)
    {
        if (!IsLocalPlayer) return;

        PlaySoundServerRpc();
    }

    [ServerRpc]
    private void PlaySoundServerRpc()
    {
        PlaySoundClientRpc();
    }

    [ClientRpc]
    private void PlaySoundClientRpc()
    {
        switch (GetComponent<RoleController>().role.Value)
        {
            case CharacterRole.Human:
                audioSource.PlayOneShot(humanSounds[Random.Range(0, humanSounds.Length)]);
                break;
            case CharacterRole.Cat:
                audioSource.PlayOneShot(catSounds[Random.Range(0, catSounds.Length)]);
                break;
        }
    }
}