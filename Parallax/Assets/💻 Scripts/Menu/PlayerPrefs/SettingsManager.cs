using System;
using UnityEngine;
using UnityEngine.UI;


public class SettingsManager : MonoBehaviour
{
	public static SettingsManager Instance;

	// EVENTS
	public event Action<float> OnMasterVolumeChanged;
	public event Action<bool> OnMuteChanged;
	public event Action<float> OnCamaraSensitivityChanged;
	public event Action<bool> OnConfineMouseChanged;
	public event Action<float> OnSFXVolumeChanged;
	public event Action<float> OnDialogueVolumeChanged;
	public event Action<float> OnMusicVolumeChanged;
	

	// PlayerPrefs keys
	private const string MasterVolumeKey = "master_volume";
	private const string MusicVolumeKey = "music_volume";
	private const string SFXVolumeKey = "sfx_volume";
	private const string DialogueVolumeKey = "dialogue_volume";

	private const string MuteKey = "mute_all";
	private const string SensitivityKey = "camara_sensitivity";
	private const string ConfineMouseKey = "confine_mouse";

	private void Awake()
	{
		Instance = this;
		LoadAll();
	}

	// SETTERS
	public void SetMasterVolume(Slider sliderValue)
	{
		PlayerPrefs.SetFloat(MasterVolumeKey, sliderValue.value);
		PlayerPrefs.Save();
		// ApplyVolume();
		OnMasterVolumeChanged?.Invoke(GetMasterVolume());
	}

	public void ToggleMute()
	{
		bool muted = !GetMute();

		PlayerPrefs.SetInt(MuteKey, muted ? 1 : 0);
		PlayerPrefs.Save();
		// ApplyVolume();
		OnMuteChanged?.Invoke(GetMute());
	}
	
	public void SetMusicVolume(Slider sliderValue)
	{ 
		PlayerPrefs.SetFloat(MusicVolumeKey, sliderValue.value);
		PlayerPrefs.Save();
		// ApplyVolume();
		OnMusicVolumeChanged?.Invoke(GetMusicVolume());
	}

	public void SetSFXVolume(Slider sliderValue)
	{
		PlayerPrefs.SetFloat(SFXVolumeKey, sliderValue.value);
		PlayerPrefs.Save();
		// ApplyVolume();
		OnSFXVolumeChanged?.Invoke(GetSFXVolume());
	}

	public void SetDialogueVolume(Slider sliderValue)
	{
		PlayerPrefs.SetFloat(DialogueVolumeKey, sliderValue.value);
		PlayerPrefs.Save();
		// ApplyVolume();
		OnDialogueVolumeChanged?.Invoke(GetDialogueVolume());
	}

	public void SetMouseSensitivity(Slider sliderValue)
	{
		PlayerPrefs.SetFloat(SensitivityKey, sliderValue.value);
		PlayerPrefs.Save();

		OnCamaraSensitivityChanged?.Invoke(sliderValue.value);
	}

	public void ToggleConfineMouse()
	{
		bool value = !GetConfineMouse();

		PlayerPrefs.SetInt(ConfineMouseKey, value ? 1 : 0);
		PlayerPrefs.Save();

		ApplyCursor();
	}

	// -------------------------
	// APPLY METHODS
	// ------------------------
	
	private void ApplyVolume()
	{
		bool muted = GetMute();
		OnMasterVolumeChanged?.Invoke(GetMasterVolume());
		OnMuteChanged?.Invoke(GetMute());
		
		OnMusicVolumeChanged?.Invoke(GetMusicVolume());
		OnSFXVolumeChanged?.Invoke(GetSFXVolume());
		OnDialogueVolumeChanged?.Invoke(GetDialogueVolume());
	}
	
	
	private void ApplyCursor()
	{
		bool confine = GetConfineMouse();
		
		Cursor.lockState = confine ? CursorLockMode.Confined : CursorLockMode.None;
		
		OnConfineMouseChanged?.Invoke(confine);
	}

	// -------------------------
	// LOAD
	// -------------------------
	private void LoadAll()
	{
		ApplyVolume();
		ApplyCursor();

		float sensitivity = GetMouseSensitivity();
		OnCamaraSensitivityChanged?.Invoke(sensitivity);
	}

	// -------------------------
	// GETTERS
	// -------------------------
	public float GetMasterVolume() =>
		PlayerPrefs.GetFloat(MasterVolumeKey, 1f);

	public bool GetMute() =>
		PlayerPrefs.GetInt(MuteKey, 0) == 1;
	
	public float GetMusicVolume() =>
		PlayerPrefs.GetFloat(MusicVolumeKey, 1f);

	public float GetSFXVolume() =>
		PlayerPrefs.GetFloat(SFXVolumeKey, 1f);

	public float GetDialogueVolume() =>
		PlayerPrefs.GetFloat(DialogueVolumeKey, 1f);

	public float GetMouseSensitivity() =>
		PlayerPrefs.GetFloat(SensitivityKey, 1f);
	
	public bool GetConfineMouse() =>
		PlayerPrefs.GetInt(ConfineMouseKey, 0) == 1;
}