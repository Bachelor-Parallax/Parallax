using Unity.Netcode;
using UnityEngine;

public class MovingPlatform : NetworkBehaviour, IActivatable
{
    [SerializeField] private Vector3 moveOffset = new Vector3(0, -5, 0);
    [SerializeField] private float speed = 2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMoving;

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + moveOffset;
    }

    private void Update()
    {
        if (!IsServer) return; 

        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );
    }

    public void Activate()
    {
        ActivateServerRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void ActivateServerRpc()
    {
        Debug.Log("Platform activated on server");
        isMoving = true;
    }
}