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
    
    Renderer[] humanRenderers;
    Renderer[] catRenderers;

    public NetworkVariable<CharacterRole> role = new(
        writePerm: NetworkVariableWritePermission.Server);

    void Awake()
    {
        human.SetActive(false);
        cat.SetActive(false);
        
        humanRenderers = human.GetComponentsInChildren<Renderer>(true);
        catRenderers = cat.GetComponentsInChildren<Renderer>(true);
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
        
        foreach (var r1 in humanRenderers)
            r1.enabled = r == CharacterRole.Human;

        foreach (var r2 in catRenderers)
            r2.enabled = r == CharacterRole.Cat;
        
        ApplyRoleSpecificPhysics(r);
        GetComponent<TemporaryMovement>().ApplyRole(r);
    }

    void ApplyRoleSpecificPhysics(CharacterRole r)
    {
        CharacterController controller = GetComponent<CharacterController>();

        if (controller == null)
            return;

        switch (role.Value)
        {
            case CharacterRole.Human:
                controller.height = 1.8f;
                controller.center = new Vector3(0, 0.9f, 0);
                controller.stepOffset = 0.4f;
                controller.slopeLimit = 45f;
                break;
            case CharacterRole.Cat:
                controller.height = 0.6f;
                controller.center = new Vector3(0, 0.3f, 0);
                controller.stepOffset = 0.2f;
                controller.slopeLimit = 60f;
                break;
            default:
                controller.height = 1.8f;
                controller.center = new Vector3(0, 0.9f, 0);
                controller.stepOffset = 0.4f;
                controller.slopeLimit = 45f;
                break;
        }
    }
}