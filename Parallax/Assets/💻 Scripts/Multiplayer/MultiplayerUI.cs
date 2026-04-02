using System.Threading.Tasks;
using UnityEngine;

public class MultiplayerUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text lobbyInfoText;
    [SerializeField] private TMPro.TMP_Text displayLobbyCode;
    [SerializeField] private TMPro.TMP_InputField lobbyCodeInput;
    [SerializeField] private GameObject levelSelectMenu;

    public void OnCreateLobbyPressed(bool isPrivate)
    {
        _ = CreateLobbyUI(isPrivate);
    }

    private async Task CreateLobbyUI(bool isPrivate)
    {
        await Multiplayer.Instance.CreateLobby(isPrivate);
        
        displayLobbyCode.SetText(
            "Lobby code: " + Multiplayer.Instance.CurrentLobbyCode
        );
    }

    // QUICK JOIN (no code)
    public void OnQuickJoinPressed()
    {
        _ = JoinLobbyUI(null);
    }

    // JOIN WITH CODE
    public void OnJoinByCodePressed()
    {
        string code = lobbyCodeInput.text.Trim().ToUpper();

        if (string.IsNullOrEmpty(code))
            _ = JoinLobbyUI(null);
        else
            _ = JoinLobbyUI(code);
    }

    private async Task JoinLobbyUI(string lobbyCode)
    {
        await Multiplayer.Instance.JoinLobby(lobbyCode);
    }

    public void OnDisconnectPressed()
    {
        _ = DisconnectUI();
    }

    private async Task DisconnectUI()
    {
        string lobbyStatusMessage = await Multiplayer.Instance.Disconnect();
        lobbyInfoText.SetText(lobbyStatusMessage);
    }

    public void OnLevelPressed(string sceneName)
    {
        SelectLevelUI(sceneName);
    }

    private void SelectLevelUI(string sceneName)
    {
        Multiplayer.Instance.LoadGameScene(sceneName);
    }
}
