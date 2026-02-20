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
        SetPlayerEnabled(primaryTarget, activePlayer == primaryTarget);
        SetPlayerEnabled(secondaryTarget, activePlayer == secondaryTarget);
    }

    private void SetPlayerEnabled(GameObject player, bool enabled)
    {
        var inputHandler = player.GetComponent<InputHandler>();
        if (inputHandler != null)
            inputHandler.enabled = enabled;

        var controller = player.GetComponent<BaseController>();
        if (controller != null)
            controller.enabled = enabled;
    }
}
