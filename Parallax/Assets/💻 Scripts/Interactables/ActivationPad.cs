using UnityEngine;

public class ActivationPad : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private bool catOnly = true;
    [SerializeField] private bool activateOnlyOnce = true;

    private bool hasActivated;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Pad hit by: " + other.name);

        if (activateOnlyOnce && hasActivated) return;

        RoleController role = other.GetComponentInParent<RoleController>();

        if (role == null)
        {
            Debug.Log("No RoleController found on " + other.name);
            return;
        }

        Debug.Log("Role: " + role.CurrentRole);

        if (catOnly && !role.IsCat)
        {
            Debug.Log("Not cat");
            return;
        }

        IActivatable activatable = target.GetComponent<IActivatable>();

        if (activatable == null)
        {
            Debug.LogWarning($"{target.name} has no IActivatable component");
            return;
        }

        Debug.Log("Activating: " + target.name);
        activatable.Activate();
        hasActivated = true;
    }
}