using UnityEngine;

public class VoteZoneTrigger : GroupActivationZone
{
    private LevelData _levelData;

    public void SetLevel(LevelData data)
    {
        _levelData = data;
    }

    protected override void OnTimerElapsed()
    {
        SceneLoader.Instance.LoadGameScene(_levelData.sceneName);
    }
}