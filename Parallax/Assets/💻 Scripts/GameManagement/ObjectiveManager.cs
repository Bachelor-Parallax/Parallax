using UnityEngine;
using System.Collections.Generic;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [Header("Debug")]
    [SerializeField] private bool logProgress = true;

    [Header("Drag checkpoints here in the order you want them collected")]
    [SerializeField] private List<Checkpoint> checkpoints = new List<Checkpoint>();

    [Header("Visibility rules")]
    [SerializeField, Min(1)] private int showNextCount = 1;

    private int currentIndex = 0;
    private int totalCheckpoints;
    private int collectedCheckpoints;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple ObjectiveManagers found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (logProgress)
            Debug.Log("ObjectiveManager initialized.");
    }

    private void Start()
    {

        checkpoints.RemoveAll(c => c == null);
        ApplyVisibility();
        LogProgress();

        collectedCheckpoints = 0;

        StatusUI.Instance.Show("Try and push the brown box!");

        totalCheckpoints = checkpoints.Count;

        Debug.Log($"ObjectiveManager Start: totalCheckpoints={totalCheckpoints}");

    }

    public void CollectCheckpoint(Checkpoint checkpoint)
    {
        if (checkpoint == null) return;

        bool isCurrentlyVisible =
            checkpoints.Contains(checkpoint) &&
            checkpoints.IndexOf(checkpoint) >= currentIndex &&
            checkpoints.IndexOf(checkpoint) < currentIndex + showNextCount;

        if (!isCurrentlyVisible)
        {
            Debug.Log($"Ignored collect for not-current checkpoint: {checkpoint.name}");
            return;
        }

        checkpoint.MarkCollected();

        currentIndex++;

        CheckpointCollected();

        ApplyVisibility();
        LogProgress();
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

        foreach (var checkpoint in checkpoints)
        {
            checkpoint.SetActive(true);
        }

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

    public void CheckpointCollected()
    {
        collectedCheckpoints++;

        switch (collectedCheckpoints)
        {
            case 1:
                StatusUI.Instance.Show("Maybe we need to use the blue to push the box? Press \"R\" to take control of the Blue.");
                break;
            case 2:
                StatusUI.Instance.Show("Great! Now push the box onto the Brown checkpoint to unlock the next checkpoint!");
                break;
            case 3:
                StatusUI.Instance.Show("Now the Green can jump onto the block! and get its checkpoint Press \"R\" to take control of Green.");
                break;
            case 4:
                StatusUI.Instance.Show("Great job! Now help the Green get to the next checkpoint! Press \"R\" to take control of the Blue again.");
                break;
            case 5:
                StatusUI.Instance.Show("Almost there! help green get to the nextcheckpoint!");
                break;
            case 6:
                StatusUI.Instance.Show("Ohh can green make the jump?!");
                break;
            case 7:
                StatusUI.Instance.Show("Yes! Now get to the next checkpoint!");
                break;
            case 8:
                StatusUI.Instance.Show("You dont always need to switch to get the next checkpoints!");
                break;
            case 9:
                StatusUI.Instance.Show("Green can stand on top of the blue to reach the next checkpoint!");
                break;
            case 10:
                StatusUI.Instance.Show("Congratulations! You've completed the tutorial! Press \"R\" to take control of the Green again.");
                break;
        }

        if (logProgress)
            Debug.Log($"CheckpointCollected: {collectedCheckpoints}/{totalCheckpoints}");

        if (collectedCheckpoints >= totalCheckpoints)
        {
            TutorialComplete();
        }
    }

    private void TutorialComplete()
    {
        Debug.Log("🎉 Tutorial complete!");
        Invoke(nameof(QuitGame), 2f);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}