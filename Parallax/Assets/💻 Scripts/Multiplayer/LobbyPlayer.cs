using Unity.Collections;
using Unity.Netcode;

public class LobbyPlayer : NetworkBehaviour
{
    public NetworkVariable<bool> IsReady =
        new NetworkVariable<bool>(false);

    public NetworkVariable<FixedString32Bytes> PlayerName =
        new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SetPlayerNameServerRpc(Multiplayer.Instance.PlayerName);
        }
    }

    [ServerRpc]
    void SetPlayerNameServerRpc(string name)
    {
        PlayerName.Value = name;
    }

    [ServerRpc]
    public void SetReadyServerRpc(bool ready)
    {
        IsReady.Value = ready;
    }
}