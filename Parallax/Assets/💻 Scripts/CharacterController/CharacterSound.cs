using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class CharacterSound : NetworkBehaviour
{
	[SerializeField] private AudioClip[] catSounds;
	[SerializeField] private AudioClip[] humanSounds;
	[SerializeField] private InputActionReference soundAction;

	private AudioSource audioSource;
	private SettingsManager _settingsManager;
	
	
	
	private void Start()
	{
		// audioSource = GetComponent<AudioSource>();
		var sources = GetComponents<AudioSource>();
		audioSource = sources[0];
		
		SceneManager.sceneLoaded += OnSceneLoaded;
		SoundSetup();
	}

	private void OnEnable()
	{
		if (soundAction == null) return;
		
		soundAction.action.performed += OnSound;
		soundAction.action.Enable();
		
		// SceneManager.sceneLoaded += OnSceneLoaded;
		//
		// // Subscribe
		// _settingsManager.OnMusicVolumeChanged += UpdateDialogueVolume;
		// _settingsManager.OnMuteChanged += UpdateMute;
	}

	private void OnDisable()
	{
		if (soundAction == null) return;

		soundAction.action.performed -= OnSound;
		soundAction.action.Disable();
		
		// SceneManager.sceneLoaded -= OnSceneLoaded;
		//
		// // Unsubscribe
		// _settingsManager.OnMusicVolumeChanged -= UpdateDialogueVolume;
		// _settingsManager.OnMuteChanged -= UpdateMute;
		
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


	private void SoundSetup() // The setup on now sceen load
	{
		// Gets the instance
		_settingsManager = SettingsManager.Instance;

		// Subscribes
		_settingsManager.OnMuteChanged += UpdateMute;
		_settingsManager.OnDialogueVolumeChanged += UpdateDialogueVolume;
		_settingsManager.OnMasterVolumeChanged += UpdateMasterVolume;
		
		// Fetches values from PlayerPrefs sync them (should only run ONCE)
		UpdateDialogueVolume(_settingsManager.GetDialogueVolume());
		UpdateMute(_settingsManager.GetMute());
	}


	// Event handling
	private void UpdateMasterVolume(float value)
	{
		UpdateDialogueVolume(_settingsManager.GetDialogueVolume());
	}
	
	private void UpdateDialogueVolume(float value) // 'value' is the value from UpdateDialogueVolume event call
	{
		Debug.LogWarning("UpdateDialogueVolume value = " + value);
		// Debug.LogWarning("Master volume = " + SettingsManager.Instance.GetMasterVolume());
		double volume = Math.Round(value * _settingsManager.GetMasterVolume(), 2);
		Debug.LogWarning("UpdateDialogueVolume total = " + volume);
		audioSource.volume = (float)volume;
	}
	
	private void UpdateMute(bool value)
	{
		Debug.LogWarning("UpdateMute" + audioSource.mute + value);
		audioSource.mute = value;
	}
	
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		SoundSetup();
	}
}