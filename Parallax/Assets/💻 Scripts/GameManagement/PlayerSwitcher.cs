using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    [SerializeField] private FollowCam followCam; 
    [SerializeField] private GameObject secondaryTarget;
    [SerializeField] private GameObject primaryTarget; 

    private GameObject activePlayer;

    void Start()
    {
        SetActivePlayer(primaryTarget);
    }  

    public void SwitchPlayer()
    {
        if (activePlayer == primaryTarget)
            SetActivePlayer(secondaryTarget);
        else
            SetActivePlayer(primaryTarget);
    }

    private void SetActivePlayer(GameObject player)
    {
        activePlayer = player;
        if (followCam)
            followCam.SetTarget(activePlayer.transform, snapToDefaultAngles: false);
    }
}
