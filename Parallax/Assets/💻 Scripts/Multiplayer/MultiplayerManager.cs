using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Random = UnityEngine.Random;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance { get; private set; }

    [Header("Lobby Settings")]
    [SerializeField] private string lobbyName = "Lobby";
    [SerializeField] private int maxPlayers = 2;
    [SerializeField] private LoadingUI loadingUI;

    [Header("Relay Settings")]
    [SerializeField] private bool dtlsSecureMode = true;

    public string PlayerId { get; private set; }
    public string PlayerName { get; private set; }

    public string CurrentLobbyCode =>
        currentLobby != null ? currentLobby.LobbyCode : "";

    private Lobby currentLobby;

    private const float k_lobbyHeartbeatInterval = 20f;
    private const float k_lobbyPollInterval = 65f;
    private const string k_keyJoinCode = "RelayJoinCode";

    private CountdownTimer heartbeatTimer = new(k_lobbyHeartbeatInterval);
    private CountdownTimer pollTimer = new(k_lobbyPollInterval);

    #region Unity Lifecycle

    private async void Start()
    {
        InitializeSingleton();
        RegisterNetworkCallbacks();
        ConfigureTimers();

        await Authenticate();
    }

    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void RegisterNetworkCallbacks()
    {
        NetworkManager.Singleton.OnTransportFailure += OnTransportFailure;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            Debug.Log("HOST STARTED");
        };
    }

    private void ConfigureTimers()
    {
        heartbeatTimer.OnTimerStop += () =>
        {
            _ = HandleHeartbeatAsync();
            heartbeatTimer.Start();
        };

        pollTimer.OnTimerStop += () =>
        {
            _ = HandlePollingAsync();
            pollTimer.Start();
        };
    }

    #endregion

    #region Authentication

    private async Task Authenticate()
    {
        await Authenticate("Player" + Random.Range(0, 1000));
    }

    private async Task Authenticate(string playerName)
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            InitializationOptions options = new();
            options.SetProfile(playerName);
            await UnityServices.InitializeAsync(options);
        }

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);
        };

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            PlayerId = AuthenticationService.Instance.PlayerId;
            PlayerName = playerName;
        }
    }

    #endregion

    #region Lobby Creation / Join

    public async Task CreateLobby(bool isPrivate)
    {
        loadingUI.Show("Creating lobby...");

        try
        {
            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

            CreateLobbyOptions options = new()
            {
                IsPrivate = isPrivate
            };

            currentLobby =
                await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            Debug.Log("Created lobby: " + currentLobby.Name + " with code " + currentLobby.LobbyCode);

            heartbeatTimer.Start();
            pollTimer.Start();

            await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id,
                new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { k_keyJoinCode, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                    }
                });

            ConfigureHostRelay(allocation);

            var tcs = new TaskCompletionSource<bool>();

            void OnServerStartedHandler()
            {
                NetworkManager.Singleton.OnServerStarted -= OnServerStartedHandler;
                tcs.SetResult(true);
            }

            NetworkManager.Singleton.OnServerStarted += OnServerStartedHandler;

            NetworkManager.Singleton.StartHost();

            await tcs.Task;

            loadingUI.Hide();

            LoadGameScene("Lobby");
        }
        catch (LobbyServiceException e)
        {
            loadingUI.Hide();
            Debug.LogError("Failed to create lobby: " + e.Message);
        }
    }

    public async Task JoinLobby(string lobbyCode = null)
    {
        loadingUI.Show("Searching for lobby...");

        try
        {
            Lobby joinedLobby = null;

            if (lobbyCode == null)
            {
                // Retry QuickJoin a few times to avoid lobby propagation delay
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        loadingUI.Show($"Searching for lobby... ({i + 1}/10)");

                        joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                        break;
                    }
                    catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.NoOpenLobbies)
                    {
                        if (i == 9)
                        {
                            loadingUI.Show("No lobby found");
                            await Task.Delay(1500);
                            throw new LobbyServiceException(LobbyExceptionReason.NoOpenLobbies, "No open lobbies found.");
                        }
                        
                        await Task.Delay(1000);
                    }
                }
            }
            else
            {
                joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            }

            currentLobby = joinedLobby;

            Debug.Log("Lobby data keys:");
            foreach (var key in currentLobby.Data.Keys)
            {
                Debug.Log(key);
            }

            pollTimer.Start();

            string relayJoinCode = currentLobby.Data[k_keyJoinCode].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            Debug.Log("Joined relay successfully");

            ConfigureClientRelay(joinAllocation);

            var tcs = new TaskCompletionSource<bool>();

            void OnClientConnectedHandler(ulong id)
            {
                if (id == NetworkManager.Singleton.LocalClientId)
                {
                    NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedHandler;
                    tcs.SetResult(true);
                }
            }

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedHandler;

            Debug.Log("Starting client...");
            NetworkManager.Singleton.StartClient();

            await tcs.Task;

            loadingUI.Hide();
        }
        catch (LobbyServiceException e)
        {
            loadingUI.Hide();

            if (e.Reason == LobbyExceptionReason.NoOpenLobbies)
            {
                Debug.Log("No open lobbies found.");
            }
            else
            {
                Debug.LogError("Failed to join lobby: " + e.Message);
            }
        }
    }

    #endregion

    #region Disconnect

    public async Task<string> Disconnect()
    {
        string result = "";

        if (currentLobby != null)
        {
            try
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
                    result = "Host left - Lobby has been deleted";
                }
                else
                {
                    await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, PlayerId);
                    result = "Client left the lobby";
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Lobby cleanup failed: " + e.Message);
                result = "Lobby cleanup failed";
            }

            currentLobby = null;
        }

        heartbeatTimer.Stop();
        pollTimer.Stop();

        NetworkManager.Singleton.Shutdown();

        await Task.Yield();
        
        SceneManager.LoadScene("MainMenu");

        return result;
    }

    #endregion

    #region Relay

    private void ConfigureHostRelay(Allocation allocation)
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>()
            .SetRelayServerData(new RelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.ConnectionData,
                allocation.ConnectionData,
                allocation.Key,
                dtlsSecureMode));
    }

    private void ConfigureClientRelay(JoinAllocation joinAllocation)
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>()
            .SetRelayServerData(new RelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData,
                joinAllocation.Key,
                dtlsSecureMode));
    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            return await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Failed to allocate relay: " + e.Message);
            throw;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            return await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Failed to get relay join code: " + e.Message);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string relayJoinCode)
    {
        try
        {
            return await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Failed to join relay: " + e.Message);
            return default;
        }
    }

    #endregion

    #region Lobby Maintenance

    private async Task HandleHeartbeatAsync()
    {
        try
        {
            await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            Debug.Log("Sent heartbeat ping to lobby: " + currentLobby.Name);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to heartbeat lobby: " + e.Message);
        }
    }

    private async Task HandlePollingAsync()
    {
        try
        {
            currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
            Debug.Log("Polled for updates on lobby: " + currentLobby.Name);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to poll for updates on lobby: " + e.Message);
        }
    }

    #endregion

    #region Scene Control

    public void LoadGameScene(string sceneName)
    {
        Debug.Log("Player Count: " + currentLobby.Players.Count);

        if (currentLobby.Players.Count > 0)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("You are alone and can not load a scene");
        }
    }

    public void ReloadCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        LoadGameScene(sceneName);
    }

    #endregion

    #region Network Callbacks

    private void OnTransportFailure()
    {
        Debug.LogError("TRANSPORT FAILED - RELAY DEAD");
    }

    private async void OnClientDisconnected(ulong clientId)
    {
        if (currentLobby == null) return;

        if (!NetworkManager.Singleton.IsHost) return;

        try
        {
            foreach (var player in currentLobby.Players)
            {
                if (player.Id == clientId.ToString())
                {
                    await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, player.Id);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to remove disconnected player: " + e.Message);
        }
    }

    private async void OnClientConnected(ulong clientId)
    {
        Debug.Log("Client connected to server: " + clientId);
        if (currentLobby != null)
        {
            currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
        }
    }

    #endregion
}