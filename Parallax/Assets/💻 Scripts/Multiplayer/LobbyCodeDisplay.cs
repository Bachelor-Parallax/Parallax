using Unity.Netcode;
using UnityEngine;

public class LobbyCodeDisplay : MonoBehaviour
{
    void OnGUI()
    {
        if (MultiplayerManager.Instance == null) return;
        
        float w = 220f, h = 40f;
        float x = 10f, y = 10f;
        //float spacing = 10f;

        string code = MultiplayerManager.Instance.CurrentLobbyCode;

        if (!string.IsNullOrEmpty(code))
        {
            GUI.Label(new Rect(x, y, 300, h), "Lobby Code: " + code);
            if (GUI.Button(new Rect(x, y + (h + 10), w, h), "Leave lobby")) _ = MultiplayerManager.Instance.Disconnect();
            if (GUI.Button(new Rect(x, y + (h + (10 + h)), w, h), "Swap Role"))
            {
                var localPlayer = NetworkManager.Singleton.LocalClient?.PlayerObject;

                if (localPlayer == null) return;

                RoleController roleController = localPlayer.GetComponent<RoleController>();

                if (roleController == null) return;

                roleController.ToggleRoleServerRpc();
            }
        }
    }
}