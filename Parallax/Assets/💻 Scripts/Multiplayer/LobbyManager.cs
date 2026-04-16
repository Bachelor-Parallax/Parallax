using Unity.Netcode;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    private string selectedLevel;
    
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
        ulong clientId = rpcParams.Receive.SenderClientId;

        Debug.Log($"Player {clientId} voted for {sceneName}");

        var playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

        if (playerObject == null)
        {
            Debug.LogError("PlayerObject missing");
            return;
        }

        var lobbyPlayer = playerObject.GetComponent<LobbyPlayer>();

        if (lobbyPlayer == null)
        {
            Debug.LogError("LobbyPlayer missing");
            return;
        }

        // mark that player ready
        lobbyPlayer.IsReady.Value = true;
        
        selectedLevel = sceneName;

        TryStartGame();
    }

    private void TryStartGame()
    {
        if (!IsServer) return;

        if (!AllPlayersReady())
        {
            Debug.LogWarning("All players are not ready");
            return;
        }
        
        Debug.Log("All players ready. Starting game...");

        CurrentLobbyState.Value = LobbyState.StartingGame;
        
        MultiplayerManager.Instance.LoadGameScene(selectedLevel);
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