using Unity.Netcode;
using UnityEngine;


public class UINetworkStart : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text displayText;
    [SerializeField] private TMPro.TMP_InputField lobbyCodeInput;

    private async void LobbyHelper(bool isPublic, string infoText)
    {
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            Debug.Log("UINetworkStart - !NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer");
            return;
        }
        
        if (Multiplayer.Instance == null)  {
            Debug.Log("UINetworkStart - Multiplayer.Instance == null");
            return;
        }
        
        await Multiplayer.Instance.CreateLobby(isPublic);
        displayText.SetText(infoText);
    }
    
    public void CreatePublicLobby()
    {
        LobbyHelper(false, "Created Public Lobby");
    }
    
    public void CreatePrivateLobby()
    {
        LobbyHelper(true, "Created Public Lobby");
    }
    
    //TODO:FIXME Hookup to helper for multiplayer check
    //TODO:FIXME Need some text feed back if you acctualy joined a lobby.
    private async void JoinPublicLobby()
    {
        await Multiplayer.Instance.JoinLobby();
        //TODO:FIXME Need some validation check here for the text
        displayText.SetText("Joined Random Lobby");
    }
    
    
    
    //TODO:FIXME Hookup to helper for multiplayer check
    
    private async void JoinPrivateLobby()
    {
        //"Join With Code"
        //OnJoinWithCode(lobbyCodeInput);

        string code = lobbyCodeInput.text;
        
        code = code.Trim().ToUpper();

        if (string.IsNullOrEmpty(code))
        {
            Debug.LogWarning("No lobby code entered.");
            return;
        }

        await Multiplayer.Instance.JoinLobby(code);
    }
}