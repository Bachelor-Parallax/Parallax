using System;
using System.Collections;
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

public class UIMultiplayer : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text lobbyInfoText;
    [SerializeField] private TMPro.TMP_Text displayLobbyCode;
    [SerializeField] private TMPro.TMP_InputField lobbyCodeInput;
    [SerializeField] private GameObject levelSelectMenu;
    
    
    
    //[SerializeField] private string lobbyName = "Lobby";
    [SerializeField] private int maxPlayers = 2;
    [SerializeField] private bool dtlsSecureMode = true;

    public static UIMultiplayer Instance { get; private set; }

    public string PlayerId { get; private set; }
    public string PlayerName { get; private set; }
    public string CurrentLobbyCode => currentLobby != null ? currentLobby.LobbyCode : "";

    private Lobby currentLobby;
    
    private const float k_lobbyHeartbeatInterval = 20f;
    private const float k_lobbyPollInterval = 65f;
    private const string k_keyJoinCode = "RelayJoinCode";

    // Instantiate timers
    private CountdownTimer heartbeatTimer = new(k_lobbyHeartbeatInterval);
    private CountdownTimer pollTimer = new(k_lobbyPollInterval);

    
    
    
    
    // ReSharper disable Unity.PerformanceAnalysis
    // ReSharper disable Unity.PerformanceAnalysis
    private async void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        await Authenticate();

        // Define timer restarts
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

        NetworkManager.Singleton.OnTransportFailure += () =>
        {
            Debug.LogError("TRANSPORT FAILED - RELAY DEAD");
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    
    
    
    
    
    private async Task Authenticate()
    {
        await Authenticate("Player" + Random.Range(0, 1000));
    }

    private async Task Authenticate(string playerName)
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            InitializationOptions options = new InitializationOptions();
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






    public void CreateLobbyFunction(bool isPrivate)
    {
        _ = CreateLobby(isPrivate);
    }
    
    private async Task CreateLobby(bool isPrivate)
    {
        try
        {
            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = isPrivate
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", maxPlayers, options);
            displayLobbyCode.SetText("Lobby code: " + currentLobby.LobbyCode);
            
            //TODO:FIXME remove commented out if it works 
            //Debug.Log("Created lobby: " + currentLobby.Name + " with code " + currentLobby.LobbyCode);

            heartbeatTimer.Start();
            pollTimer.Start();

            await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { k_keyJoinCode, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.ConnectionData,
                allocation.ConnectionData,
                allocation.Key,
                dtlsSecureMode)
            );

            NetworkManager.Singleton.OnServerStarted += OnHostStarted;

            NetworkManager.Singleton.StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to create lobby: " + e.Message);
        }
    }

    
    
    
    
    
    
    public void QuickJoinLobbyFunction()
    {
        _ = QuickJoinLobby();
    }
    
    //TODO:FIXME better naming ()
    private async Task QuickJoinLobby()
    {
        try
        {
            currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            pollTimer.Start();

            string relayJoinCode = currentLobby.Data[k_keyJoinCode].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData,
                joinAllocation.Key,
                dtlsSecureMode));

            NetworkManager.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to quick join lobby: " + e.Message);
        }
    }
    
    
    
    
    
    
    
    public void JoinLobbyByCodeFunction()
    {
        _ = JoinLobbyByCode(lobbyCodeInput.text.ToUpper());
    }
    private async Task JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            pollTimer.Start();

            string relayJoinCode = currentLobby.Data[k_keyJoinCode].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>()
                .SetRelayServerData(new RelayServerData(
                    joinAllocation.RelayServer.IpV4,
                    (ushort)joinAllocation.RelayServer.Port,
                    joinAllocation.AllocationIdBytes,
                    joinAllocation.ConnectionData,
                    joinAllocation.HostConnectionData,
                    joinAllocation.Key,
                    dtlsSecureMode));

            NetworkManager.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to join lobby by code: " + e.Message);
        }
    }

    
    
    
    
    
    
    public async void Disconnect()
    {
        if (currentLobby != null)
        {
            try
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    // If host leaves → delete entire lobby
                    await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
                    lobbyInfoText.SetText("Host left - Lobby has been deleted");
                }
                else
                {
                    // Normal client leaves
                    await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, PlayerId);
                    lobbyInfoText.SetText("Client left - Lobby has been deleted");
                }
            }
            catch (Exception e)
            {
                //TODO:FIXME maby remove the debug
                Debug.LogWarning("Lobby cleanup failed: " + e.Message);
                lobbyInfoText.SetText("Lobby cleanup failed: " + e.Message);
            }

            currentLobby = null;
        }

        heartbeatTimer.Stop();
        pollTimer.Stop();

        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Lobby");
    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation =
                await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
            return allocation;
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
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
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
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Failed to join relay: " + e.Message);
            return default;
        }
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
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
            Debug.Log("Polled for updates on lobby: " + currentLobby.Name);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to poll for updates on lobby: " + e.Message);
        }
    }

    
    
    
    private void OnHostStarted()
    {
        Debug.Log("Host fully started, loading scene...");

        NetworkManager.Singleton.SceneManager.LoadScene(
            "Thea",
            LoadSceneMode.Single
        );

        StartCoroutine(LogSceneAfterDelay());
        NetworkManager.Singleton.OnServerStarted -= OnHostStarted;
    }

    
    
    
    private async void OnClientDisconnected(ulong clientId)
    {
        if (currentLobby == null) return;

        // Only host should manage lobby membership cleanup
        if (!NetworkManager.Singleton.IsHost) return;

        try
        {
            // Find matching lobby player
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

    
    
    
    private IEnumerator LogSceneAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Scene after 1 second: " + SceneManager.GetActiveScene().name);
    }
}