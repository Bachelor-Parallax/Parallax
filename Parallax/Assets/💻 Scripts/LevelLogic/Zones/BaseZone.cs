using Unity.Netcode;
using UnityEngine;

public abstract class BaseZone : NetworkBehaviour
{
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
        if (!IsValidPlayer(other.gameObject)) return;
        OnPlayerEnter(other.gameObject);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!IsValidPlayer(other.gameObject)) return;
        OnPlayerExit(other.gameObject);
    }

    private bool IsValidPlayer(GameObject obj)
    {
        if (!obj.CompareTag(GameConstants.PLAYER_TAG)) return false;
        Debug.Log(obj.tag);
        if (TryGetComponent(out NetworkObject netObj))
        {
            Debug.Log("Is owner: " + netObj.IsOwner);
            return netObj.IsOwner;
        }
        return false;
    }
}