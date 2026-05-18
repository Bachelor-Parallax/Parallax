using Unity.Netcode;
using UnityEngine;

public class PlayerRoleStats : NetworkBehaviour
{
    [Header("Human")]
    [SerializeField] private float humanMoveSpeed = 5f;
    [SerializeField] private float humanSprintSpeed = 8f;
    [SerializeField] private float humanGravity = -18f;
    [SerializeField] private float humanJumpHeight = 0.8f;

    [Header("Cat")]
    [SerializeField] private float catMoveSpeed = 8f;
    [SerializeField] private float catSprintSpeed = 12f;
    [SerializeField] private float catGravity = -14f;
    [SerializeField] private float catJumpHeight = 1.8f;

    private PlayerMovement movement;
    private PlayerJumpAbility jumpAbility;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        jumpAbility = GetComponent<PlayerJumpAbility>();
    }

    public void ApplyRole(CharacterRole role)
    {
        switch (role)
        {
            case CharacterRole.Human:
                ApplyHumanStats();
                break;

            case CharacterRole.Cat:
                ApplyCatStats();
                break;
        }
    }

    private void ApplyHumanStats()
    {
        movement.SetMoveSpeed(humanMoveSpeed);
        movement.SetSprintSpeed(humanSprintSpeed);
        movement.SetGravity(humanGravity);

        if (jumpAbility != null)
            jumpAbility.SetJumpHeight(humanJumpHeight);
    }

    private void ApplyCatStats()
    {
        movement.SetMoveSpeed(catMoveSpeed);
        movement.SetSprintSpeed(catSprintSpeed);
        movement.SetGravity(catGravity);

        if (jumpAbility != null)
            jumpAbility.SetJumpHeight(catJumpHeight);
    }
}