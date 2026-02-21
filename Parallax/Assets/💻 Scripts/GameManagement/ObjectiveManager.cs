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
        totalCheckpoints = Object.FindObjectsByType<Checkpoint>(FindObjectsSortMode.None).Length;
        collectedCheckpoints = 0;
        StatusUI.Instance.Show($"Collect {totalCheckpoints} checkpoints");

        if (logProgress)
            Debug.Log($"Objective: Collect {totalCheckpoints} checkpoints");
    }

    public void CheckpointCollected()
    {
        collectedCheckpoints++;

        if (logProgress)
            Debug.Log($"Progress: {collectedCheckpoints}/{totalCheckpoints}");
            

        StatusUI.Instance.Show("Push the box onto the brown platform");


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