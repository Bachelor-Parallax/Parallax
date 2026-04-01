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

    public NetworkVariable<CharacterRole> role = new();

    void UpdateRole(CharacterRole r)
    {
        cat.SetActive(r == CharacterRole.Cat);
        human.SetActive(r == CharacterRole.Human);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (NetworkManager.Singleton.IsHost)
            UpdateRole(CharacterRole.Human);
        else if (NetworkManager.Singleton.IsClient)
            UpdateRole(CharacterRole.Cat);
    }
}
