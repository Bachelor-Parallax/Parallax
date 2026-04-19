using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class LobbyServiceWrapper : MonoBehaviour
{
    public static LobbyServiceWrapper Instance { get; private set; }

    [SerializeField] private string lobbyName;

    public Lobby currentLobby { get; private set; }
    private CountdownTimer heartbeatTimer = new(k_lobbyHeartbeatInterval);
    private CountdownTimer pollTimer = new(k_lobbyPollInterval);
    private const string k_keyJoinCode = "RelayJoinCode";

    private const float k_lobbyHeartbeatInterval = 20f;
    private const float k_lobbyPollInterval = 65f;
    private string relayJoinCode;
    
    private void Awake()
    {
        InitializeSingleton();
        ConfigureTimers();
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

    public async Task<Lobby> CreateLobby(bool isPrivate, int maxPlayers)
    {
        try
        {
            Allocation allocation = await RelayServiceWrapper.Instance.AllocateRelay(maxPlayers);
            string relayJoinCode = await RelayServiceWrapper.Instance.GetRelayJoinCode(allocation);

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

            RelayServiceWrapper.Instance.ConfigureHostRelay(allocation);

            var tcs = new TaskCompletionSource<bool>();

            void OnServerStartedHandler()
            {
                NetworkManager.Singleton.OnServerStarted -= OnServerStartedHandler;
                tcs.SetResult(true);
            }

            NetworkManager.Singleton.OnServerStarted += OnServerStartedHandler;

            NetworkManager.Singleton.StartHost();

            await tcs.Task;
            
            return currentLobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to create lobby: " + e.Message);
            throw;
        }
    }

    public async Task<Lobby> JoinLobby(string lobbyCode)
    {
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
                        joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                        break;
                    }
                    catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.NoOpenLobbies)
                    {
                        if (i == 9)
                        {
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

            JoinAllocation joinAllocation = await RelayServiceWrapper.Instance.JoinRelay(relayJoinCode);
            Debug.Log("Joined relay successfully");

            RelayServiceWrapper.Instance.ConfigureClientRelay(joinAllocation);

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
            return currentLobby;
        }
        catch (LobbyServiceException e)
        {
            if (e.Reason == LobbyExceptionReason.NoOpenLobbies)
            {
                Debug.Log("No open lobbies found.");
            }
            else
            {
                Debug.LogError("Failed to join lobby: " + e.Message);
            }

            throw;
        }
    }

    public async Task<string> Disconnect(string playerId)
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
                    await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerId);
                    result = $"Client: {playerId} - left the lobby";
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
}
