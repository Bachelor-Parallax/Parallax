using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public NetworkVariable<LobbyState> CurrentLobbyState =
        new NetworkVariable<LobbyState>(LobbyState.Waiting);

    private List<LobbyPlayer> players = new();

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            RegisterExistingPlayers();
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void RegisterExistingPlayers()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            SpawnLobbyPlayer(client.ClientId);
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;

        SpawnLobbyPlayer(clientId);
    }

    private void SpawnLobbyPlayer(ulong clientId)
    {
        GameObject playerObj = Instantiate(
            NetworkManager.Singleton.NetworkConfig.PlayerPrefab
        );

        playerObj.GetComponent<NetworkObject>()
            .SpawnAsPlayerObject(clientId);

        players.Add(playerObj.GetComponent<LobbyPlayer>());
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (!IsServer) return;

        players.RemoveAll(p => p.OwnerClientId == clientId);
    }

    public void ToggleReady(bool ready)
    {
        NetworkObject playerObj =
            NetworkManager.Singleton.LocalClient.PlayerObject;

        if (playerObj == null) return;

        playerObj.GetComponent<LobbyPlayer>()
            .SetReadyServerRpc(ready);
    }

    public void StartGame()
    {
        if (!IsServer) return;

        if (!AllPlayersReady())
        {
            Debug.Log("Not all players ready");
            return;
        }

        CurrentLobbyState.Value = LobbyState.StartingGame;

        Multiplayer.Instance.LoadGameScene("GameScene");
    }

    private bool AllPlayersReady()
    {
        foreach (var player in players)
        {
            if (!player.IsReady.Value)
                return false;
        }

        return true;
    }
}