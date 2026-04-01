using System.Collections;
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

    void Awake()
    {
        human.SetActive(false);
        cat.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        role.OnValueChanged += OnRoleChanged;

        if (IsServer)
        {
            role.Value = (OwnerClientId == NetworkManager.ServerClientId)
                ? CharacterRole.Human
                : CharacterRole.Cat;
        }

        StartCoroutine(ApplyRoleNextFrame());
    }
    
    IEnumerator ApplyRoleNextFrame()
    {
        yield return null;
        UpdateRole(role.Value);
    }

    void OnRoleChanged(CharacterRole previous, CharacterRole current)
    {
        UpdateRole(current);
    }

    void UpdateRole(CharacterRole r)
    {
        human.SetActive(r == CharacterRole.Human);
        cat.SetActive(r == CharacterRole.Cat);
        Debug.Log($"Role {r} | Human active: {human.activeSelf} | Cat active: {cat.activeSelf}");
    }
}