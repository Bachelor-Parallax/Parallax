using System.Collections.Generic;
using UnityEngine;

public class ActivationPad : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private GameObject[] targets;

    [Header("Rules")]
    [SerializeField] private bool catOnly = false;
    [SerializeField] private bool requireTwoPlayers = false;
    [SerializeField] private bool activateOnlyOnce = true;

    private bool hasActivated;

    private HashSet<GameObject> playersOnPad = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        RoleController role = other.GetComponentInParent<RoleController>();
        if (role == null) return;

        if (catOnly && !role.IsCat) return;

        GameObject player = role.gameObject;

        if (!playersOnPad.Contains(player))
        {
            playersOnPad.Add(player);
            Debug.Log("Player entered pad. Count: " + playersOnPad.Count);
        }

        TryActivate();
    }

    private void OnTriggerExit(Collider other)
    {
        RoleController role = other.GetComponentInParent<RoleController>();
        if (role == null) return;

        GameObject player = role.gameObject;

        if (playersOnPad.Contains(player))
        {
            playersOnPad.Remove(player);
            Debug.Log("Player left pad. Count: " + playersOnPad.Count);
        }
    }

    private void TryActivate()
    {
        if (activateOnlyOnce && hasActivated)
            return;

        if (requireTwoPlayers && playersOnPad.Count < 2)
        {
            Debug.Log("Not enough players on pad");
            return;
        }

        bool activatedSomething = false;

        foreach (GameObject target in targets)
        {
            if (target == null)
                continue;

            IActivatable activatable = target.GetComponent<IActivatable>();

            if (activatable == null)
            {
                Debug.LogWarning($"{target.name} has no IActivatable component");
                continue;
            }

            Debug.Log("Activating: " + target.name);
            activatable.Activate();
            activatedSomething = true;
        }

        if (activatedSomething)
            hasActivated = true;
    }
}