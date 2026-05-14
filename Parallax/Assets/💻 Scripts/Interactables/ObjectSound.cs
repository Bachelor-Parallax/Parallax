using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ObjectSound : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;

    private AudioSource audioSource;
    private SettingsManager settingsManager;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        SoundSetup();
    }

    private void OnDisable()
    {
        if (settingsManager == null) return;

        settingsManager.OnMuteChanged -= UpdateMute;
        settingsManager.OnSFXVolumeChanged -= UpdateSFXVolume;
        settingsManager.OnMasterVolumeChanged -= UpdateMasterVolume;
    }

    public void PlayRandomPitch()
    {
        if (audioClip == null) return;

        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(audioClip);
    }

    private void SoundSetup()
    {
        if (settingsManager != null)
        {
            settingsManager.OnMuteChanged -= UpdateMute;
            settingsManager.OnSFXVolumeChanged -= UpdateSFXVolume;
            settingsManager.OnMasterVolumeChanged -= UpdateMasterVolume;
        }

        settingsManager = SettingsManager.Instance;

        if (settingsManager == null) return;

        settingsManager.OnMuteChanged += UpdateMute;
        settingsManager.OnSFXVolumeChanged += UpdateSFXVolume;
        settingsManager.OnMasterVolumeChanged += UpdateMasterVolume;

        UpdateSFXVolume(settingsManager.GetSFXVolume());
        UpdateMute(settingsManager.GetMute());
    }

    private void UpdateMasterVolume(float value)
    {
        UpdateSFXVolume(settingsManager.GetSFXVolume());
    }

    private void UpdateSFXVolume(float value)
    {
        double volume = Math.Round(value * settingsManager.GetMasterVolume(), 2);
        audioSource.volume = (float)volume;
    }

    private void UpdateMute(bool value)
    {
        audioSource.mute = value;
    }
}