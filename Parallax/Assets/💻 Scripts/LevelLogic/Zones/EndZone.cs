using UnityEngine;

public class EndZone : GroupActivationZone
{
    protected override void OnTimerElapsed()
    {
        Debug.Log("Level Complete");
    }
}