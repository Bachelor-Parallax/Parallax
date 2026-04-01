using Unity.Netcode;
using UnityEngine;

public enum CharacterRole
{
    Human,
    Cat
}

public class RoleController : NetworkBehaviour
{
    public GameObject human;
    public GameObject cat;

    public NetworkVariable<CharacterRole> role = new(
        writePerm: NetworkVariableWritePermission.Server);

    void UpdateRole(CharacterRole r)
    {
        cat.SetActive(r == CharacterRole.Cat);
        human.SetActive(r == CharacterRole.Human);
    }

    public override void OnNetworkSpawn()
    {
        role.OnValueChanged += OnRoleChanged;

        if (IsServer)
        {
            if (OwnerClientId == NetworkManager.Singleton.LocalClientId)
                role.Value = CharacterRole.Human;
            else
                role.Value = CharacterRole.Cat;
        }
    }

    void OnRoleChanged(CharacterRole previous, CharacterRole current)
    {
        UpdateRole(current);
    }
}
