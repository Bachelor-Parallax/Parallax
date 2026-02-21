using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputRouter inputRouter;
    [SerializeField] private FollowCam followCam;
    [SerializeField] private GameObject PlayerA;
    [SerializeField] private GameObject PlayerB;
    private IMovement activeMovement;

    private GameObject active;

    void Start()
    {
        SetActive(PlayerA); 
    }

    public void OnSwitch(InputValue value)
    {
        if (!value.isPressed) return;

        Debug.Log("R pressed → switching player");

        if (active == PlayerA)
            SetActive(PlayerB);
        else
            SetActive(PlayerA);
    }

    private void SetActive(GameObject player)
    {
        // ✅ stop previous player (this is to prevent continued movement if the player was mid-input when switching) 
        //TODO: not sure if this is the best way to do it, but it works for now. We could also consider adding a "Stop" method to the IMovement interface for a cleaner solution.
        activeMovement?.Move(Vector2.zero);

        active = player;

        activeMovement = active.GetComponent<IMovement>();
        inputRouter.SetActive(active);

        if (followCam) followCam.SetTarget(active.transform, snapToDefaultAngles: false);

        Debug.Log("Active player is now: " + active.name);
    }
}