using Unity.Collections;
using Unity.Netcode;

public class LobbyPlayer : NetworkBehaviour
{
    public NetworkVariable<bool> IsReady =
        new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    public NetworkVariable<FixedString32Bytes> PlayerName =
        new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SetPlayerNameServerRpc(MultiplayerManager.Instance.PlayerName);
        }
    }

    [ServerRpc]
    void SetPlayerNameServerRpc(string name)
    {
        PlayerName.Value = new FixedString32Bytes(name ?? "Player");
    }

    [ServerRpc]
    public void SetReadyServerRpc(bool ready)
    {
        IsReady.Value = ready;
    }
    
    
}