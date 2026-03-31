using UnityEngine;

public class UILobbyCode : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text displayLobbyInfo;

    // Helper Function to check that the multiplayer "I.e manager" still exists
    private void MultiplayerInstanceCheck()
    {
        if (Multiplayer.Instance == null)  {
            Debug.Log("UILobbyCode - Multiplayer.Instance == null");
            return;
        }
    }
    
    public void MakeLobbyAndDisplayCode()
    {
        MultiplayerInstanceCheck();
        string lobbyCode = Multiplayer.Instance.CurrentLobbyCode;
        displayLobbyInfo.SetText("Lobby Code: " + lobbyCode);
    }

    public void LeaveLobbyAndDisplayLeft()
    {
        MultiplayerInstanceCheck();
        Multiplayer.Instance.Disconnect();
        displayLobbyInfo.SetText("You left the lobby");
    }
}
