using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class BaseZone : MonoBehaviour
{
    protected List<GameObject> PlayersInZone = new();

    /// <summary>
    /// Called when a player enters the zone, after updating the player list
    /// </summary>
    protected virtual void OnPlayerEnter(GameObject player)
    { }

    /// <summary>
    /// Called when a player exits the zone, after updating the player list
    /// </summary>
    protected virtual void OnPlayerExit(GameObject player)
    { }

    public void OnTriggerEnter(Collider other)
    {
        if (!IsLocalPlayer(other.gameObject)) return;
        if (!PlayersInZone.Contains(other.gameObject)) PlayersInZone.Add(other.gameObject);
        OnPlayerEnter(other.gameObject);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!IsLocalPlayer(other.gameObject)) return;
        if (PlayersInZone.Contains(other.gameObject)) PlayersInZone.Remove(other.gameObject);
        OnPlayerExit(other.gameObject);
    }

    private bool IsLocalPlayer(GameObject obj)
    {
        if (!obj.CompareTag(GameConstants.PLAYER_TAG)) return false;
        if (TryGetComponent(out NetworkObject netObj))
        {
            return netObj.IsLocalPlayer;
        }
        return false;
    }
}