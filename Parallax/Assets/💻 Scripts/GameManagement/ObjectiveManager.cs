using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [Header("Debug")]
    [SerializeField] private bool logProgress = true;

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
        collectedCheckpoints = 0;

        if (CheckpointManager.Instance == null)
        {
            Debug.LogError("ObjectiveManager: No CheckpointManager in scene!");
            totalCheckpoints = 0;
            return;
        }

        totalCheckpoints = CheckpointManager.Instance.Total;

        Debug.Log($"ObjectiveManager Start: totalCheckpoints={totalCheckpoints}");

        StatusUI.Instance.Show("Try and collect the checkpoints! Start by pushing the box onto the brown platform to help the Green jump onto the block.");
    }

    public void CheckpointCollected()
    {
        collectedCheckpoints++;

        switch (collectedCheckpoints)
        {
            case 1:
                StatusUI.Instance.Show("Push the box onto the brown platform");
                break;
            case 2:
                StatusUI.Instance.Show("Now the Green can jump onto the block! and get its checkpoint Press \"R\" to take control of Green.");
                break;
            case 3:
                StatusUI.Instance.Show("Great job! Now help the Green get to the next checkpoint! Press \"R\" to take control of the Blue again.");
                break;
            case 4:
                StatusUI.Instance.Show("Almost there! Just one more checkpoint to go!");
                break;
            case 5:
                StatusUI.Instance.Show("Last checkpoint! You can do it!");
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