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
        if (playerA == null)
            return;
        Debug.Log("Swap Requested");
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;

        if (playerCount == 1)
        {
            Debug.Log("One player on server");
            if (IsServer)
                ToggleRole(playerA);
            else
                ToggleRoleServerRpc(playerA.NetworkObjectId);

            return;
        }

        if (playerB == null)
            return;

        Debug.Log("Multiple players on server");
        
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
        Debug.Log("Swapping roles of clients");
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
    
    [ServerRpc]
    private void ToggleRoleServerRpc(ulong playerId)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out var obj))
            return;

        RoleController player = obj.GetComponent<RoleController>();
        if (player == null)
            return;

        ToggleRole(player);
    }

    private void SwapRoles(RoleController a, RoleController b)
    {
        (a.role.Value, b.role.Value) = (b.role.Value, a.role.Value);

        Debug.Log($"Swapped roles: {a.OwnerClientId} ↔ {b.OwnerClientId}");
    }
    
    private void ToggleRole(RoleController player)
    {
        Debug.Log("Forcing swap");
        player.role.Value = player.role.Value == CharacterRole.Cat
            ? CharacterRole.Human
            : CharacterRole.Cat;

        Debug.Log($"Solo role toggle for {player.OwnerClientId}");
    }
}