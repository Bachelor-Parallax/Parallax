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
	[SerializeField] private AudioClip[] jumpSounds;
	[SerializeField] private AudioClip[] footstepSounds;
	[SerializeField] private AudioSource dialogueAudioSource;
	[SerializeField] private AudioSource sfxAudioSource;	

	private PlayerAudioEvents playerAudioEvents;
	private SettingsManager _settingsManager;
	
	
	
	private void Awake()
	{
		playerAudioEvents = GetComponent<PlayerAudioEvents>();
	}

	private void OnEnable()
	{
		if (playerAudioEvents != null)
		{
			playerAudioEvents.OnCharacterSound += RequestCharacterSound;
			playerAudioEvents.OnJump += RequestJumpSound;
			playerAudioEvents.OnFootstep += RequestFootstepSound;
		}

		SceneManager.sceneLoaded += OnSceneLoaded;
		SoundSetup();
	}

	private void OnDisable()
	{
		if (playerAudioEvents != null)
		{
			playerAudioEvents.OnCharacterSound -= RequestCharacterSound;
			playerAudioEvents.OnJump -= RequestJumpSound;
			playerAudioEvents.OnFootstep -= RequestFootstepSound;
		}

		if (_settingsManager != null)
		{
			_settingsManager.OnMuteChanged -= UpdateMute;
			_settingsManager.OnDialogueVolumeChanged -= UpdateDialogueVolume;
			_settingsManager.OnSFXVolumeChanged -= UpdateSFXVolume;
			_settingsManager.OnMasterVolumeChanged -= UpdateMasterVolume;
		}

    SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void RequestCharacterSound()
	{
		if (!IsLocalPlayer) return;
		PlaySoundServerRpc();
	}

	private void RequestJumpSound()
	{
		if (!IsLocalPlayer) return;
		PlayJumpServerRpc();
	}

	private void RequestFootstepSound()
	{
		if (!IsLocalPlayer) return;
		PlayFootstepServerRpc();
	}

	[ServerRpc]
	private void PlayJumpServerRpc()
	{
		PlayJumpClientRpc();
	}

	[ClientRpc]
	private void PlayJumpClientRpc()
	{
		sfxAudioSource.PlayOneShot(jumpSounds[Random.Range(0, jumpSounds.Length)]);
	}

	[ClientRpc]
	private void PlayFootstepClientRpc()
	{
		sfxAudioSource.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)]);
	}

	[ServerRpc]
	private void PlayFootstepServerRpc()
	{
		PlayFootstepClientRpc();
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
				dialogueAudioSource.PlayOneShot(humanSounds[Random.Range(0, humanSounds.Length)]);
				break;
			case CharacterRole.Cat:
				dialogueAudioSource.PlayOneShot(catSounds[Random.Range(0, catSounds.Length)]);
				break;
		}
	}


	private void SoundSetup()
	{
		if (_settingsManager != null)
		{
			_settingsManager.OnMuteChanged -= UpdateMute;
			_settingsManager.OnDialogueVolumeChanged -= UpdateDialogueVolume;
			_settingsManager.OnSFXVolumeChanged -= UpdateSFXVolume;
			_settingsManager.OnMasterVolumeChanged -= UpdateMasterVolume;
		}

		_settingsManager = SettingsManager.Instance;

		if (_settingsManager == null) return;

		_settingsManager.OnMuteChanged += UpdateMute;
		_settingsManager.OnDialogueVolumeChanged += UpdateDialogueVolume;
		_settingsManager.OnSFXVolumeChanged += UpdateSFXVolume;
		_settingsManager.OnMasterVolumeChanged += UpdateMasterVolume;

		UpdateDialogueVolume(_settingsManager.GetDialogueVolume());
		UpdateSFXVolume(_settingsManager.GetSFXVolume());
		UpdateMute(_settingsManager.GetMute());
	}


	// Event handling
	private void UpdateMasterVolume(float value)
	{
		UpdateDialogueVolume(_settingsManager.GetDialogueVolume());
		UpdateSFXVolume(_settingsManager.GetSFXVolume());
	}
	
	private void UpdateDialogueVolume(float value) // 'value' is the value from UpdateDialogueVolume event call
	{
		Debug.LogWarning("UpdateDialogueVolume value = " + value);
		// Debug.LogWarning("Master volume = " + SettingsManager.Instance.GetMasterVolume());
		double volume = Math.Round(value * _settingsManager.GetMasterVolume(), 2);
		Debug.LogWarning("UpdateDialogueVolume total = " + volume);
		dialogueAudioSource.volume = (float)volume;
	}

	private void UpdateSFXVolume(float value) // 'value' is the value from UpdateSFXVolume event call
	{
		Debug.LogWarning("UpdateSFXVolume value = " + value);
		double volume = Math.Round(value * _settingsManager.GetMasterVolume(), 2);
		Debug.LogWarning("UpdateSFXVolume total = " + volume);
		sfxAudioSource.volume = (float)volume;
	}
	
	private void UpdateMute(bool value)
	{
		Debug.LogWarning("UpdateMute" + dialogueAudioSource.mute + value);
		dialogueAudioSource.mute = value;
		Debug.LogWarning("UpdateMute" + sfxAudioSource.mute + value);
		sfxAudioSource.mute = value;
	}
	
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		SoundSetup();
	}
}