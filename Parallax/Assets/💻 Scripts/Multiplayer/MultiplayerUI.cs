using System.Threading.Tasks;
using UnityEngine;

public class MultiplayerUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField lobbyCodeInput;

    public void OnCreateLobbyPressed(bool isPrivate)
    {
        Debug.Log("Create lobby button pressed");
        // _ = CreateLobbyUI(isPrivate);
        RunTask(CreateLobbyUI(isPrivate));
    }

    private async Task CreateLobbyUI(bool isPrivate)
    {
        await MultiplayerManager.Instance.CreateLobby(isPrivate);
    }

    // QUICK JOIN (no code)
    public void OnQuickJoinPressed()
    {
        Debug.Log("Quick join button pressed");
        _ = JoinLobbyUI(null);
    }

    // JOIN WITH CODE
    public void OnJoinByCodePressed()
    {
        Debug.Log("Join by code button pressed");
        string code = lobbyCodeInput.text.Trim().ToUpper();

        if (string.IsNullOrEmpty(code))
            _ = JoinLobbyUI(null);
        else
            _ = JoinLobbyUI(code);
    }

    private async Task JoinLobbyUI(string lobbyCode)
    {
        await MultiplayerManager.Instance.JoinLobby(lobbyCode);
    }
    
    private async void RunTask(Task task)
    {
        try
        {
            await task;
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }
}
