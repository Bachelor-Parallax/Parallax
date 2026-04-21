using UnityEngine;

public class EndZone : GroupActivationZone
{
    protected override void OnTimerElapsed()
    {
        SceneLoader.Instance.LoadGameScene(GameConstants.LOBBY_SCENE_NAME);
    }
}