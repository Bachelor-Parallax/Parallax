using UnityEngine;

public class EndZone : GroupActivationZone
{
    [SerializeField] private LevelData _levelData;

    protected override void OnTimerElapsed()
    {
        LevelManager.Instance.CompleteLevel();
    }
}