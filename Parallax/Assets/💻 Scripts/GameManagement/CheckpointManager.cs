using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [Header("Drag checkpoints here in the order you want them collected")]
    [SerializeField] private List<Checkpoint> checkpoints = new List<Checkpoint>();

    [Header("Visibility rules")]
    [SerializeField, Min(1)] private int showNextCount = 1;

    private int currentIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        checkpoints.RemoveAll(c => c == null);
        ApplyVisibility();
        LogProgress();
    }

    public void CollectCheckpoint(Checkpoint checkpoint)
    {
        if (checkpoint == null) return;

        // Only allow collecting currently visible checkpoints
        bool isCurrentlyVisible =
            checkpoints.Contains(checkpoint) &&
            checkpoints.IndexOf(checkpoint) >= currentIndex &&
            checkpoints.IndexOf(checkpoint) < currentIndex + showNextCount;

        if (!isCurrentlyVisible)
        {
            Debug.Log($"Ignored collect for not-current checkpoint: {checkpoint.name}");
            return;
        }

        // Mark it as collected
        checkpoint.MarkCollected();

        currentIndex++;

        ObjectiveManager.Instance?.CheckpointCollected();

        ApplyVisibility();
        LogProgress();
    }

    private void ApplyVisibility()
    {
        for (int i = 0; i < checkpoints.Count; i++)
        {
            bool shouldBeActive =
                i >= currentIndex &&
                i < currentIndex + showNextCount;

            checkpoints[i].SetActive(shouldBeActive);
        }
    }

    private void LogProgress()
    {
        Debug.Log($"Checkpoints: {currentIndex}/{checkpoints.Count} collected. Showing next {showNextCount}.");
    }

    public int Total => checkpoints.Count;
    public int Collected => Mathf.Clamp(currentIndex, 0, checkpoints.Count);
    public bool IsComplete => Collected >= Total;

    public void ResetProgress()
    {
        currentIndex = 0;

        ApplyVisibility();
        LogProgress();
    }
}