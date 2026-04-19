using UnityEngine;

public class EndZone : BaseZone
{
    protected override void OnPlayerEnter(GameObject player)
    {
        if (PlayersInZone.Count >= 2)
        {
            // complete the level
        }
    }
}