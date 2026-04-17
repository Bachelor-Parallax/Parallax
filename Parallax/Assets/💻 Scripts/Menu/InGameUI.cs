using System.Threading.Tasks;
using UnityEngine;

public class InGameUI : MonoBehaviour
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
        await MultiplayerManager.Instance.Disconnect();
    }
}
