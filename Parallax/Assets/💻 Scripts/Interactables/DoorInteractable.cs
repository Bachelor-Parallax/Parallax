using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [Header("Sliding Door Settings")]
    [SerializeField] private float slideDistance = 4f;
    [SerializeField] private float speed = 4f;
    [SerializeField] private Vector3 slideDirection = Vector3.right;

    [Header("Door Requirements")]
    [SerializeField] private KeyInteractable requiredKey;

    private bool isOpen;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    private void Awake()
    {
        closedPosition = transform.localPosition;
        openPosition = closedPosition + slideDirection.normalized * slideDistance;
    }

    private void Update()
    {
        Vector3 targetPosition = isOpen ? openPosition : closedPosition;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * speed
        );
    }

    public void Interact(GameObject interactor)
    {
        if (requiredKey != null && !requiredKey.IsCollected)
        {
            Debug.Log("You need a key to open this door.");
            return;
        }

        isOpen = !isOpen;
        Debug.Log($"Door interacted by: {interactor.name}");
    }

    public string GetInteractText()
    {
        if (requiredKey != null && !requiredKey.IsCollected)
            return "Door is locked. A key is required.";

        return isOpen ? "Press [E] to close door" : "Press [E] to open door";
    }
}