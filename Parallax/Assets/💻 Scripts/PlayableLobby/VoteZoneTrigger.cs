using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class VoteZoneTrigger : NetworkBehaviour
{
    public LevelData levelData;

    private HashSet<ulong> voters = new HashSet<ulong>();

    private Coroutine countdownCoroutine;
    
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

        EnterZoneServerRpc();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        var netObj = other.GetComponent<NetworkObject>();

        if (netObj == null || !netObj.IsOwner)
            return;

        ExitZoneServerRpc();
    }

    [ServerRpc]
    private void EnterZoneServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        voters.Add(clientId);

        Debug.Log($"Player {clientId} voted for {levelData.sceneName}");

        TryStartCountdown();
    }

    [ServerRpc]
    private void ExitZoneServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        voters.Remove(clientId);

        CancelCountdown();
    }

    private void TryStartCountdown()
    {
        if (!IsServer)
            return;

        int totalPlayers = NetworkManager.Singleton.ConnectedClientsList.Count;

        if (voters.Count != totalPlayers)
            return;

        if (countdownCoroutine != null)
            return;

        countdownCoroutine = StartCoroutine(StartCountdown());
    }

    private void CancelCountdown()
    {
        if (!IsServer)
            return;

        if (countdownCoroutine == null)
            return;

        StopCoroutine(countdownCoroutine);
        countdownCoroutine = null;

        Debug.Log("Countdown cancelled");
    }

    private IEnumerator StartCountdown()
    {
        Debug.Log($"All players voted for {levelData.sceneName}. Starting in 5 seconds.");

        yield return new WaitForSeconds(5f);

        SceneLoader.Instance.LoadGameScene(levelData.sceneName);

        countdownCoroutine = null;
    }
    
    // public LevelData levelData;
    //
    // public void SetLevel(LevelData data)
    // {
    //     levelData = data;
    // }
    //
    // private void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log("OnTriggerEnter fired");
    //     if (!other.CompareTag("Player"))
    //         return;
    //     
    //     var netObj = other.GetComponent<NetworkObject>();
    //
    //     if (netObj == null || !netObj.IsOwner)
    //         return;
    //
    //     Debug.Log($"Player {netObj.OwnerClientId} voted for {levelData.sceneName}");
    //
    //     LobbyManager.Instance.VoteLevelServerRpc(levelData.sceneName);
    // }
    //
    // private void OnTriggerExit(Collider other)
    // {
    //     Debug.Log("OnTriggerExit fired");
    //     if (!other.CompareTag("Player"))
    //         return;
    //     
    //     var netObj = other.GetComponent<NetworkObject>();
    //
    //     if (netObj == null || !netObj.IsOwner)
    //         return;
    //     
    //     LobbyManager.Instance.ToggleReady(false);
    // }
}
