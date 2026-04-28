using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayServiceWrapper : PersistentSingleton<RelayServiceWrapper>
{
    [SerializeField] private bool dtlsSecureMode = true;

    public async Task<Allocation> AllocateRelay(int maxPlayers)
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

    public async Task<JoinAllocation> JoinRelay(string relayJoinCode)
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

    public async Task<string> GetRelayJoinCode(Allocation allocation)
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

    public void ConfigureHostRelay(Allocation allocation)
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

    public void ConfigureClientRelay(JoinAllocation joinAllocation)
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
}
