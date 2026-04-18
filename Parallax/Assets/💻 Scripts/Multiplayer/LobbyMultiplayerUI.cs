using System;
using System.Threading.Tasks;
using UnityEngine;

public class LobbyMultiplayerUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text lobbyInfoText;
    [SerializeField] private TMPro.TMP_Text lobbyCodeText;

    private void Start()
    {
        string lobbyCode = MultiplayerManager.Instance.CurrentLobbyCode;
        lobbyCodeText.SetText($"Lobby Code: {lobbyCode}");
    }

    public void OnDisconnectPressed()
    {
        _ = DisconnectUI();
    }

    private async Task DisconnectUI()
    {
        string lobbyStatusMessage = await MultiplayerManager.Instance.Disconnect();
        lobbyInfoText.SetText(lobbyStatusMessage);
    }

    public void OnLevelPressed(string sceneName)
    {
        SelectLevelUI(sceneName);
    }

    private void SelectLevelUI(string sceneName)
    {
        SceneLoader.Instance.LoadGameScene(sceneName);
    }
}
