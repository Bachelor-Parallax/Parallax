using Unity.Netcode;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public NetworkVariable<LobbyState> CurrentLobbyState =
        new NetworkVariable<LobbyState>(LobbyState.Waiting);

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log("Client connected to lobby: " + clientId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log("Client disconnected from lobby: " + clientId);
    }

    public void ToggleReady(bool ready)
    {
        var playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;

        if (playerObject == null) return;

        playerObject
            .GetComponent<LobbyPlayer>()
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
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObj = client.PlayerObject;

            if (playerObj == null) return false;

            var lobbyPlayer = playerObj.GetComponent<LobbyPlayer>();

            if (!lobbyPlayer.IsReady.Value)
                return false;
        }

        return true;
    }
}