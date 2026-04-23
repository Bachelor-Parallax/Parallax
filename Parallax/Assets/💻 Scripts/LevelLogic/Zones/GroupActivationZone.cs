using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Timers;
using System.Collections;

public abstract class GroupActivationZone : BaseZone
{
    [SerializeField] private int countdownSeconds = 3;
    [SerializeField] private TMPro.TMP_Text voteText;
    private readonly HashSet<ulong> _players = new();
    private Coroutine _countdownCoroutine = null;
    
    private void Start()
    {
        if (voteText != null)
            voteText.alpha = 0f;
    }

    /// <summary>
    /// Will be invoked when all players have been in the zone for duration of the countdown
    /// </summary>
    protected abstract void OnTimerElapsed();

    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        OnTimerElapsed();
    }

    protected override void OnPlayerEnter(GameObject player)
    {
        Debug.Log("Player enter");
        EnterZoneServerRpc();
    }

    protected override void OnPlayerExit(GameObject player)
    {
        ExitZoneServerRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void EnterZoneServerRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        _players.Add(clientId);

        // check if countdown may start
        if (!IsServer) return;
        if (_players.Count != GameConstants.MAX_PLAYERS) return;
        if (_countdownCoroutine != null) return;

        // start the countdown
        Debug.Log("Countdown start");
        _countdownCoroutine = StartCoroutine(StartCountdown());
        StartCountdownClientRpc(countdownSeconds);
        if (voteText != null)
            UpdateUIClientRpc(_players.Count);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void ExitZoneServerRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        _players.Remove(clientId);

        // check cancel conditions
        if (!IsServer) return;
        if (_countdownCoroutine == null) return;

        // cancel countdown
        Debug.Log("Countdown cancel");
        StopCoroutine(_countdownCoroutine);
        _countdownCoroutine = null;
        if (voteText != null)
            UpdateUIClientRpc(_players.Count);
        StopCountdownClientRpc();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void StartCountdownClientRpc(int seconds)
    {
        StartCoroutine(ClientCountdown(seconds));
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void StopCountdownClientRpc()
    {
        StopAllCoroutines(); // simple and fine here
    }
    
    private IEnumerator ClientCountdown(int seconds)
    {
        int remaining = seconds;

        while (remaining > 0)
        {
            if (voteText != null)
                voteText.text = $"Starting in {remaining}";
            yield return new WaitForSeconds(1f);
            remaining--;
        }
    }

    private IEnumerator StartCountdown()
    {
        yield return new WaitForSeconds(countdownSeconds);
        OnTimerElapsed();
        _countdownCoroutine = null;
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateUIClientRpc(int count)
    {  
        if (count == 0)
        {
            voteText.alpha = 0f;
            return;
        }

        voteText.alpha = 1f;
        voteText.text = $"{count}/{GameConstants.MAX_PLAYERS}";
    }
}