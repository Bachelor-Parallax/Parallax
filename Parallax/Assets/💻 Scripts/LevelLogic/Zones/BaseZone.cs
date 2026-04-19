using Unity.Netcode;
using UnityEngine;

public abstract class BaseZone : MonoBehaviour
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
        if (!IsLocalPlayer(other.gameObject)) return;
        OnPlayerEnter(other.gameObject);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!IsLocalPlayer(other.gameObject)) return;
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