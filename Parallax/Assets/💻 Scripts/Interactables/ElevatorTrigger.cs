using System.Collections.Generic;
using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    public List<Transform> PlayersOnElevator = new();

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger hit: " + other.name);

        CharacterController controller =
            other.GetComponentInParent<CharacterController>();

        if (controller == null)
        {
            Debug.Log("No CharacterController found on: " + other.name);
            return;
        }

        Transform player = controller.transform;

        if (!PlayersOnElevator.Contains(player))
        {
            PlayersOnElevator.Add(player);
            Debug.Log("Player entered elevator: " + player.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterController controller =
            other.GetComponent<CharacterController>();

        if (controller == null)
            controller = other.GetComponentInParent<CharacterController>();

        if (controller == null)
            return;

        PlayersOnElevator.Remove(controller.transform);

        Debug.Log("Player left elevator");
    }
}