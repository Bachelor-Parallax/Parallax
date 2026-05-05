using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] songs;
    private AudioSource audioSource;
    private SettingsManager _settingsManager;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        _settingsManager = SettingsManager.Instance;
        
        // Subscribes
        _settingsManager.OnMuteChanged += UpdateMute;
        _settingsManager.OnMusicVolumeChanged += UpdateMusicVolume;
        _settingsManager.OnMasterVolumeChanged += UpdateMasterVolume;
    }

    private void Start()
    {
        // Fetches values from PlayerPrefs sync them (should only run ONCE)
        UpdateMusicVolume(_settingsManager.GetMusicVolume());
        UpdateMute(_settingsManager.GetMute());
        
        PlayRandomSong();
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

        Invoke(nameof(PlayRandomSong), clip.length); // Free from pooling (no constant checking)
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

    // private void FixedUpdate()
    // {
    //     if (!audioSource.isPlaying)
    //     {
    //         PlayRandomSong();
    //     }
    // }
    //
    // public void PlayRandomSong()
    // {
    //     if (songs.Length == 0)
    //     {
    //         Debug.LogWarning("No songs assigned!");
    //         return;
    //     }
    //
    //     int randomIndex = Random.Range(0, songs.Length);
    //     audioSource.clip = songs[randomIndex];
    //     audioSource.Play();
    // }
}