using UnityEngine;

public class VoteZoneTrigger : GroupActivationZone
{
    public LevelData levelData;

    public void SetLevel(LevelData data)
    {
        levelData = data;
    }

    protected override void OnTimerElapsed()
    {
        Debug.Log("Countdown complete, loading: " + levelData.sceneName);
        SceneLoader.Instance.LoadGameScene(levelData.sceneName);
    }
}