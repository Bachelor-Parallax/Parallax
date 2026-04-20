using System.Threading.Tasks;
using Unity.Netcode;

public class InGameUI : NetworkBehaviour
{
    public void OnResumePressed()
    {
        gameObject.SetActive(false);
    }

    public void OnSettingsPressed()
    {
        
    }

    public void OnGuidePressed()
    {
        
    }

    public void OnRetryPressed()
    {
        SceneLoader.Instance.ReloadCurrentScene();
    }

    public void OnLeaveGamePressed()
    {
        _ = LeaveGame();
    }

    private async Task LeaveGame()
    {
        if (!IsServer)
            await MultiplayerManager.Instance.Disconnect();
        else
            SceneLoader.Instance.LoadGameScene("PlayableLobby");
    }
}
