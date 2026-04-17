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
    [SerializeField] private int maxPlayers = 2;
    [SerializeField] private LoadingUI loadingUI;
    [SerializeField] private string LobbySceneName = "PlayableLobby";
    
    public string PlayerId { get; private set; }
    public string PlayerName { get; private set; }

    public string CurrentLobbyCode =>
        currentLobby != null ? currentLobby.LobbyCode : "";

    private Lobby currentLobby;

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

    #endregion

    #region Authentication

    private async Task Authenticate()
    {
        PlayerId = await AuthenticationServiceWrapper.Instance.Authenticate();
    }

    private async Task Authenticate(string playerName)
    {
        PlayerId = await AuthenticationServiceWrapper.Instance.Authenticate(playerName);
    }

    #endregion

    #region Lobby Creation / Join / Disconnect

    public async Task CreateLobby(bool isPrivate)
    {
        loadingUI.Show("Creating lobby...");

        try
        {
            currentLobby = await LobbyServiceWrapper.Instance.CreateLobby(isPrivate, maxPlayers);

            loadingUI.Hide();

            SceneLoader.Instance.LoadGameScene(LobbySceneName);
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

    public async Task<string> Disconnect()
    {
        return await LobbyServiceWrapper.Instance.Disconnect(PlayerId);
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