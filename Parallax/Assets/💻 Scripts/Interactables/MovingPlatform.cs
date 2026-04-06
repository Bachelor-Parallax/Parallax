using UnityEngine;

public class MovingPlatform : MonoBehaviour, IActivatable
{
    [Header("Movement")]
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float speed = 2f;

    private Vector3 startPosition;
    private bool isMoving = false;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (!isMoving) return;
        Debug.Log("Moving platform towards " + targetPosition);
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );
    }

    public void Activate()
    {
        isMoving = true;
    }
}