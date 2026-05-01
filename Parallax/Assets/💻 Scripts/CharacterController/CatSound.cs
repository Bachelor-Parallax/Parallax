using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class CatSound : NetworkBehaviour
{
    [SerializeField] private AudioClip[] catSounds;
    [SerializeField] private InputActionReference catSoundAction;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (catSoundAction == null) return;

        catSoundAction.action.performed += OnCatSound;
        catSoundAction.action.Enable();
    }

    private void OnDisable()
    {
        if (catSoundAction == null) return;

        catSoundAction.action.performed -= OnCatSound;
        catSoundAction.action.Disable();
    }

    private void OnCatSound(InputAction.CallbackContext context)
    {
        if (!IsLocalPlayer) return;
        PlayRandomCatSound();
    }

    public void PlayRandomCatSound()
    {
        if (catSounds == null || catSounds.Length == 0)
        {
            Debug.LogWarning("No cat sounds assigned!");
            return;
        }

        int randomIndex = Random.Range(0, catSounds.Length);
        audioSource.PlayOneShot(catSounds[randomIndex]);
    }
}