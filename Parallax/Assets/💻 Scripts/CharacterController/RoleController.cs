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

    public CharacterRole CurrentRole => role.Value;

    public bool IsHuman => role.Value == CharacterRole.Human;
    public bool IsCat => role.Value == CharacterRole.Cat;

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
        GetComponent<Movement>().ApplyRole(r);

        // var asymObjects = FindObjectsOfType<AsymVisibility>(true);
        // foreach (var asym in asymObjects)
        // {
        //     asym.ApplyRole(r);
        // }
    }

    void ApplyRoleSpecificPhysics(CharacterRole r)
    {
        CharacterController controller = GetComponent<CharacterController>();
        Debug.Log($"Before physics apply | Role: {r} | Pos: {transform.position} | Height: {controller.height} | Center: {controller.center}");

        if (controller == null)
            return;

        switch (r)
        {
            case CharacterRole.Human:
                controller.height = 2f;
                controller.center = new Vector3(0, 0f, 0);
                controller.stepOffset = 0.4f;
                controller.slopeLimit = 45f;
                break;
            case CharacterRole.Cat:
                controller.height = 1f;
                controller.center = new Vector3(0, 0f, 0);
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
        Debug.Log($"After physics apply | Role: {r} | Pos: {transform.position} | Height: {controller.height} | Center: {controller.center}");
    }
}