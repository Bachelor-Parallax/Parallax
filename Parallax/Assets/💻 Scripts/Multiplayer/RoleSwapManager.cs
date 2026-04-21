using Unity.Netcode;
using UnityEngine;

public class RoleSwapManager : NetworkBehaviour
{
    public static RoleSwapManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void RequestSwap(RoleController playerA, RoleController playerB)
    {
        Debug.Log("Role Swap Request");
        if (playerA == null || playerB == null)
            return;

        if (IsServer)
            SwapRoles(playerA, playerB);
        else
            RequestSwapServerRpc(
                playerA.NetworkObjectId,
                playerB.NetworkObjectId);
    }

    [ServerRpc]
    private void RequestSwapServerRpc(
        ulong playerAId,
        ulong playerBId)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerAId, out var objA))
            return;

        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerBId, out var objB))
            return;

        RoleController a = objA.GetComponent<RoleController>();
        RoleController b = objB.GetComponent<RoleController>();

        if (a == null || b == null)
            return;

        SwapRoles(a, b);
    }

    private void SwapRoles(RoleController a, RoleController b)
    {
        (a.role.Value, b.role.Value) = (b.role.Value, a.role.Value);

        Debug.Log($"Swapped roles: {a.OwnerClientId} ↔ {b.OwnerClientId}");
    }
}