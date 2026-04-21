using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Timers;

public abstract class GroupActivationZone : BaseZone
{
    [SerializeField] private int _countdownSeconds = 3;
    private readonly HashSet<ulong> _players = new HashSet<ulong>();
    private Timer _timer;

    private void Awake()
    {
        _timer = new Timer(_countdownSeconds * 1000);
        _timer.Elapsed += OnTimedEvent;
        _timer.AutoReset = false;
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
        Debug.Log("1");

        Debug.Log($"Players in zone: {_players.Count} of {GameConstants.MAX_PLAYERS}");

        if (_players.Count != GameConstants.MAX_PLAYERS) return;
        Debug.Log("2");

        if (_timer.Enabled) return;

        // start the countdown
        Debug.Log("Countdown start");
        _timer.Start();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void ExitZoneServerRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        _players.Remove(clientId);

        // check cancel conditions
        if (!IsServer) return;
        if (!_timer.Enabled) return;

        // cancel countdown
        Debug.Log("Countdown cancel");
        _timer.Stop();
    }
}