using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] songs;
    private AudioSource audioSource;
    private SettingsManager _settingsManager;
    private bool isPaused;

    private void Start()
    {

        audioSource = GetComponent<AudioSource>();
        _settingsManager = SettingsManager.Instance;
        
        // Subscribes
        _settingsManager.OnMuteChanged += UpdateMute;
        _settingsManager.OnMusicVolumeChanged += UpdateMusicVolume;
        _settingsManager.OnMasterVolumeChanged += UpdateMasterVolume;
        // Fetches values from PlayerPrefs sync them (should only run ONCE)
        UpdateMusicVolume(_settingsManager.GetMusicVolume());
        UpdateMute(_settingsManager.GetMute());
        
        PlayRandomSong();
    }
    
    private void Update()
    {
        if (!audioSource.isPlaying && !isPaused)
        {
            PlayRandomSong();
        }
    }
    
    public void PlayRandomSong()
    {
        if (songs.Length == 0)
        {
            Debug.LogWarning("No songs assigned!");
            return;
        }

        int randomIndex = Random.Range(0, songs.Length);
        AudioClip clip = songs[randomIndex];

        audioSource.clip = clip;
        audioSource.Play();

        // Invoke(nameof(PlayRandomSong), clip.length); // Free from pooling (no constant checking)
    }

    public void TogglePause()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            isPaused = true;
        }
        else if (isPaused)
        {
            audioSource.UnPause();
            isPaused = false;
        }
    }
    
    // Event handling
    private void UpdateMasterVolume(float volume)
    {
        UpdateMusicVolume(_settingsManager.GetMusicVolume());
    }
    
    private void UpdateMusicVolume(float value) // 'value' is the value from UpdateMusicVolume event call
    {
        Debug.LogWarning("UpdateMusicVolume value = " + value);
        double volume = Math.Round(value * _settingsManager.GetMasterVolume(), 2);
        Debug.LogWarning("UpdateMusicVolume total = " + volume);
        audioSource.volume = (float)volume;
    }
    
    private void UpdateMute(bool value)
    {
        Debug.LogWarning("UpdateMute x3" + audioSource.mute + value);
        audioSource.mute = value;
    }
}