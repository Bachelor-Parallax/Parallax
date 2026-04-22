using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Timers;
using System.Collections;

public abstract class GroupActivationZone : BaseZone
{
    [SerializeField] private int _countdownSeconds = 3;
    private readonly HashSet<ulong> _players = new();
    private Coroutine _countdownCoroutine = null;

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
    }

    private IEnumerator StartCountdown()
    {
        yield return new WaitForSeconds(_countdownSeconds);
        OnTimerElapsed();
        _countdownCoroutine = null;
    }
}