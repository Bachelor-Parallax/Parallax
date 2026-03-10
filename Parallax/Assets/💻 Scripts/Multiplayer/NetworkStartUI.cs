using UnityEngine;
using Unity.Netcode;

public class NetworkStartUI : MonoBehaviour
{
    private string lobbyCodeInput = "";

    void OnGUI()
    {
        float w = 220f, h = 40f;
        float x = 10f, y = 10f;
        float spacing = 10f;

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUI.Button(new Rect(x, y, w, h), "Create Public Lobby"))
                OnHostPublic();

            if (GUI.Button(new Rect(x, y + (h + spacing), w, h), "Create Private Lobby"))
                OnHostPrivate();

            if (GUI.Button(new Rect(x, y + 2 * (h + spacing), w, h), "Join Random Lobby"))
                OnJoinQuick();

            // Text input field
            lobbyCodeInput = GUI.TextField(
                new Rect(x, y + 3 * (h + spacing), w, h),
                lobbyCodeInput
            );

            if (GUI.Button(new Rect(x, y + 4 * (h + spacing), w, h), "Join With Code"))
                OnJoinWithCode(lobbyCodeInput);
        }
    }

    private void OnHostPublic()
    {
        Multiplayer.Instance.CreateLobby(false);
    }

    private void OnHostPrivate()
    {
        Multiplayer.Instance.CreateLobby(true);
    }

    private void OnJoinQuick()
    {
        Multiplayer.Instance.QuickJoinLobby();
    }

    private void OnJoinWithCode(string code)
    {
        code = code.Trim().ToUpper();

        if (string.IsNullOrEmpty(code))
        {
            Debug.LogWarning("No lobby code entered.");
            return;
        }

        Multiplayer.Instance.JoinLobbyByCode(code);
    }
}
