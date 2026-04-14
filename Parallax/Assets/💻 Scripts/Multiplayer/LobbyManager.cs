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

        if (playerObject == null)
        {
            Debug.LogWarning("Local PlayerObject missing");
            return;
        }

        var lobbyPlayer = playerObject.GetComponent<LobbyPlayer>();

        if (lobbyPlayer == null)
        {
            Debug.LogError("LobbyPlayer component missing!");
            return;
        }

        lobbyPlayer.SetReadyServerRpc(ready);
    }

    // CLIENT → SERVER request to start the game
    [ServerRpc(RequireOwnership = false)]
    public void VoteLevelServerRpc(string sceneName, ServerRpcParams rpcParams = default)
    {
        Debug.Log($"Client {rpcParams.Receive.SenderClientId} voted for {sceneName}");

        ToggleReady(true);
        StartGame(sceneName);
    }

    public void StartGame(string sceneName)
    {
        Debug.Log($"StartGame called for {sceneName}");

        if (!IsServer)
        {
            Debug.Log("Not server, ignoring StartGame");
            return;
        }

        if (!AllPlayersReady())
        {
            Debug.Log("Not all players ready");
            return;
        }

        CurrentLobbyState.Value = LobbyState.StartingGame;

        Debug.Log($"Multiplayer.Instance = {MultiplayerManager.Instance}");
        MultiplayerManager.Instance.LoadGameScene(sceneName);
    }

    private bool AllPlayersReady()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObj = client.PlayerObject;

            if (playerObj == null)
            {
                Debug.LogWarning("PlayerObject is null");
                return false;
            }

            var lobbyPlayer = playerObj.GetComponent<LobbyPlayer>();

            if (lobbyPlayer == null)
            {
                Debug.LogError($"LobbyPlayer missing on {playerObj.name}");
                return false;
            }

            if (!lobbyPlayer.IsReady.Value)
            {
                Debug.Log($"Player {client.ClientId} is not ready");
                return false;
            }
        }

        return true;
    }
}