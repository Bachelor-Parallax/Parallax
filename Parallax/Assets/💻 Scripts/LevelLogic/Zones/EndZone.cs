using UnityEngine;

public class EndZone : GroupActivationZone
{
    protected override void OnTimerElapsed()
    {
        LevelManager.Instance.CompleteLevel();
    }
}