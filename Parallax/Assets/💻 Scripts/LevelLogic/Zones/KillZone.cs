using System;
using System.Data;
using UnityEngine;

public class KillZone : BaseZone
{
    [Flags]
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

        if (player.TryGetComponent(out RoleController rc))
        {
            Role flag = rc.CurrentRole switch
            {
                CharacterRole.Cat => Role.Cat,
                CharacterRole.Human => Role.Human,
                _ => throw new Exception("Invalid role")
            };

            if ((_activeRoles & flag) != 0)
            {
                // TODO: display GUI for all players with cause of death, and prompt to restart or quit
                GetComponent<Collider>().enabled = false;
                LevelManager.Instance.RestartLevel();
            }
        }
    }
}