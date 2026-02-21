using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private CollectorType allowedCollector;
    private bool collected = false;

   private void OnTriggerEnter(Collider other)
{
    if (collected) return;

    var identity = other.GetComponentInParent<CollectorIdentity>();

    if (identity == null) return;

    if (identity.collectorType != allowedCollector)
    {
        Debug.Log("This checkpoint is not for you!");
        return;
    }

    Collect();
}

    private void Collect()
    {
        collected = true;

        Debug.Log("Checkpoint collected by " + allowedCollector);

        ObjectiveManager.Instance.CheckpointCollected();

        gameObject.SetActive(false);
    }
}