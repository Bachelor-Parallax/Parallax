using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MovingPlatform : NetworkBehaviour, IActivatable
{
    [SerializeField] private Vector3 moveOffset = new Vector3(0, -5, 0);
    [SerializeField] private float speed = 2f;

    [SerializeField] private ElevatorTrigger elevatorTrigger;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    private bool isMoving;

    private readonly List<Transform> passengers = new();

    private void Awake()
    {
        if (elevatorTrigger == null)
            elevatorTrigger = GetComponentInChildren<ElevatorTrigger>();
    }

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + moveOffset;
    }

    public void Activate()
    {
        if (!IsServer) return;

        AttachPlayers();

        isMoving = true;
    }

    private void Update()
    {
        if (!IsServer || !isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isMoving = false;

            DetachPlayers();
        }
    }

    private void AttachPlayers()
    {
        passengers.Clear();

        if (elevatorTrigger == null)
            return;

        foreach (Transform player in elevatorTrigger.PlayersOnElevator)
        {
            if (player == null) continue;

            Movement movement = player.GetComponent<Movement>();
            if (movement != null)
                movement.MovementLocked = true;

            NetworkObject netObj = player.GetComponent<NetworkObject>();
            if (netObj != null)
                netObj.TrySetParent(transform, true);

            passengers.Add(player);

            Debug.Log("Attached player: " + player.name);
        }
    }

    private void DetachPlayers()
    {
        foreach (Transform passenger in passengers)
        {
            if (passenger == null) continue;

            Movement movement = passenger.GetComponent<Movement>();
            if (movement != null)
                movement.MovementLocked = false;

            NetworkObject netObj = passenger.GetComponent<NetworkObject>();
            if (netObj != null)
                netObj.TryRemoveParent(true);
            else
                passenger.SetParent(null, true);
        }

        passengers.Clear();
    }
}