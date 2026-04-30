using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] songs;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PlayRandomSong();
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
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
        audioSource.clip = songs[randomIndex];
        audioSource.Play();
    }
}