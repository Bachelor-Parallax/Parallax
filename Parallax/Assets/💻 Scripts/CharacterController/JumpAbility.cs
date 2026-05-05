using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

using UnityEngine.SceneManagement;

public class JumpAbility : MonoBehaviour
{
    private Movement movement;
    private CharacterController controller;
    
    [SerializeField] private AudioClip[] jumpSounds;
    [SerializeField] private InputActionReference jumpAction;
    
    private AudioSource audioSource;
    private SettingsManager _settingsManager;
    
    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        movement = GetComponent<Movement>();
        controller = GetComponent<CharacterController>();
        // audioSource = GetComponent<AudioSource>();
        var sources = GetComponents<AudioSource>();
        audioSource = sources[1];
    }

    private void Start()
    {
        if (!movement.IsOwner) return;

        if (jumpAction != null)
        {
            jumpAction.action.Enable();
            jumpAction.action.performed += HandleJump;
        }
    }

    // void OnEnable()
    // {
    //     if (jumpAction != null)
    //     {
    //         jumpAction.action.Enable();
    //         jumpAction.action.performed += HandleJump;
    //     }
    // }

    void OnDisable()
    {
        if (jumpAction != null)
        {
            jumpAction.action.performed -= HandleJump;
            jumpAction.action.Disable();
        }
    }

    // void Update()
    // {
    //     if (!enabled) return;
    //     if (movement == null || controller == null) return;
    //     if (jumpAction == null) return;
    //
    //     if (jumpAction.action.WasPressedThisFrame() && controller.isGrounded)
    //     {
    //         movement.SetVerticalVelocity(Mathf.Sqrt(movement.JumpHeight * -2f * movement.Gravity));
    //
    //         if (jumpSounds.Length > 0)
    //         {
    //             int index = Random.Range(0, jumpSounds.Length);
    //             audioSource.pitch = Random.Range(0.9f, 1.1f);
    //             audioSource.PlayOneShot(jumpSounds[index]);
    //         }
    //     }
    // }
    
    private void HandleJump(InputAction.CallbackContext ctx)
    {
        if (!movement.IsOwner) return;
        if (!controller.isGrounded) return;

        movement.SetVerticalVelocity(Mathf.Sqrt(movement.JumpHeight * -2f * movement.Gravity));

        if (jumpSounds.Length > 0)
        {
            int index = Random.Range(0, jumpSounds.Length);
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(jumpSounds[index]);
        }
    }
    
    
    
    private void SoundSetup() // The setup on now sceen load
    {
        // Gets the instance
        _settingsManager = SettingsManager.Instance;

        // Subscribes
        _settingsManager.OnMuteChanged += UpdateMute;
        _settingsManager.OnSFXVolumeChanged += UpdateSFXVolume;
        _settingsManager.OnMasterVolumeChanged += UpdateMasterVolume;
        
        // Fetches values from PlayerPrefs sync them (should only run ONCE)
        UpdateSFXVolume(_settingsManager.GetSFXVolume());
        UpdateMute(_settingsManager.GetMute());
    }
    
    // Event handling
    private void UpdateMasterVolume(float value)
    {
        UpdateSFXVolume(_settingsManager.GetSFXVolume());
    }
    
    private void UpdateSFXVolume(float value) // 'value' is the value from UpdateSFXVolume event call
    {
        Debug.LogWarning("UpdateSFXVolume value = " + value);
        double volume = Math.Round(value * _settingsManager.GetMasterVolume(), 2);
        Debug.LogWarning("UpdateSFXVolume total = " + volume);
        audioSource.volume = (float)volume;
    }
    
    private void UpdateMute(bool value)
    {
        Debug.LogWarning("UpdateMute x2" + audioSource.mute + value);
        audioSource.mute = value;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SoundSetup();
    }
}