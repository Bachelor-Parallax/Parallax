using System;
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
        if (!other.CompareTag("Player")) return;
        
        Debug.Log($"Player voted for {levelData.levelName}");
        
        LobbyManager.Instance.VoteLevelServerRpc(levelData.sceneName);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        LobbyManager.Instance.ToggleReady(false);
        Debug.Log($"Player un-voted for {levelData.levelName}");
    }
}
