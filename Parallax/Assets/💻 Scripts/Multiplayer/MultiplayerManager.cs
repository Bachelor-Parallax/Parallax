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
    [SerializeField] private string LobbySceneName = "PlayableLobby";

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
        // ConfigureTimers();

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

    // private void ConfigureTimers()
    // {
    //     heartbeatTimer.OnTimerStop += () =>
    //     {
    //         _ = HandleHeartbeatAsync();
    //         heartbeatTimer.Start();
    //     };
    //
    //     pollTimer.OnTimerStop += () =>
    //     {
    //         _ = HandlePollingAsync();
    //         pollTimer.Start();
    //     };
    // }

    #endregion

    #region Authentication

    private async Task Authenticate()
    {
        PlayerId = "";
        Debug.Log(AuthenticationServiceWrapper.Instance);
        PlayerId = await AuthenticationServiceWrapper.Instance.Authenticate();
    }

    private async Task Authenticate(string playerName)
    {
        PlayerId = "";
        PlayerId = await AuthenticationServiceWrapper.Instance.Authenticate(playerName);
    }

    #endregion

    #region Lobby Creation / Join

    public async Task CreateLobby(bool isPrivate)
    {
        loadingUI.Show("Creating lobby...");

        try
        {
            currentLobby = await LobbyServiceWrapper.Instance.CreateLobby(isPrivate, maxPlayers);

            loadingUI.Hide();

            LoadGameScene(LobbySceneName);
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
            currentLobby = await LobbyServiceWrapper.Instance.JoinLobby(lobbyCode);
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

    #region Scene Control

    public void LoadGameScene(string sceneName)
    {
        Debug.Log("Player Count: " + currentLobby.Players.Count);

        if (currentLobby.Players.Count > 0)
        {
            SceneLoader.Instance.LoadGameScene(sceneName);
        }
        else
        {
            Debug.Log("You are alone and can not load a scene");
        }
    }

    public void ReloadCurrentScene()
    {
        SceneLoader.Instance.ReloadCurrentScene();
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