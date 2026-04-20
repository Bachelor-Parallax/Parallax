using UnityEngine;

public class KillZone : BaseZone
{
    [System.Flags]
    private enum Role
    {
        Cat = 1,
        Human = 2
    }

    [SerializeField] private string _contactMessage;
    [SerializeField][Tooltip("The role types that will die in the kill zone")] private Role _activeRoles;

    protected override void OnPlayerEnter(GameObject player)
    {
        Debug.Log(_contactMessage);
        // TODO: display GUI for all players with cause of death, and prompt to restart or quit
    }
}