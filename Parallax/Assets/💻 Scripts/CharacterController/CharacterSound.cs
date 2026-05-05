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
		audioSource = GetComponent<AudioSource>();
		SceneManager.sceneLoaded += OnSceneLoaded;
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


	public void SoundSetup() // The setup on now sceen load
	{
		// Gets the instance
		_settingsManager = SettingsManager.Instance;

		// Subscribes
		_settingsManager.OnMuteChanged += UpdateMute;
		_settingsManager.OnDialogueVolumeChanged += UpdateDialogueVolume;
		
		// Fetches values from PlayerPrefs sync them (should only run ONCE)
		UpdateDialogueVolume(_settingsManager.GetDialogueVolume());
		UpdateMute(_settingsManager.GetMute());
	}


	// Event handling
	void UpdateDialogueVolume(float value) // 'value' is the value from UpdateDialogueVolume event call
	{
		// Debug.LogWarning("UpdateMusic value = " + value);
		// Debug.LogWarning("Master volume = " + SettingsManager.Instance.GetMasterVolume());
		double volume = Math.Round(value * _settingsManager.GetMasterVolume(), 2);
		// Debug.LogWarning("Volume = " + volume);
		audioSource.volume = (float)volume;
	}
	
	void UpdateMute(bool value)
	{
		// Debug.LogWarning("UpdateMute" + audioSource.mute + value);
		audioSource.mute = value;
	}
	
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		// Unsubscribe from any old event
		_settingsManager.OnMusicVolumeChanged -= UpdateDialogueVolume;
		_settingsManager.OnMuteChanged -= UpdateMute;
		
		SoundSetup();
	}
}