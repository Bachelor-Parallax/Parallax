using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Who can collect this checkpoint?")]
    [SerializeField] private CollectorType requiredType;

    public CollectorType RequiredType => requiredType;

    public bool IsCollected { get; private set; }

    public bool IsActive => gameObject.activeSelf && !IsCollected;

    private void OnTriggerEnter(Collider other)
    {
        if (IsCollected) return;

        var identity = other.transform.root.GetComponent<CollectorIdentity>();
        if (identity == null) return;

        if (identity.collectorType != requiredType) return;

        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.CollectCheckpoint(this);
        }
    }

    public void MarkCollected()
    {
        IsCollected = true;
        gameObject.SetActive(false);
    }

    // This is called by the manager to show/hide checkpoints based on progress
    public void SetActive(bool active)
    {
        gameObject.SetActive(active && !IsCollected);
    }
}