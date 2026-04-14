using Unity.Netcode;
using UnityEngine;

public class VoteZoneTrigger : MonoBehaviour
{
    public LevelData levelData;
    
    public void SetLevel(LevelData data)
    {
        levelData = data;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        var netObj = other.GetComponent<NetworkObject>();

        if (netObj == null || !netObj.IsOwner)
            return;

        Debug.Log($"Player {netObj.OwnerClientId} voted for {levelData.sceneName}");

        LobbyManager.Instance.VoteLevelServerRpc(levelData.sceneName);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        var netObj = other.GetComponent<NetworkObject>();

        if (netObj == null || !netObj.IsOwner)
            return;
        
        LobbyManager.Instance.ToggleReady(false);
    }
}
