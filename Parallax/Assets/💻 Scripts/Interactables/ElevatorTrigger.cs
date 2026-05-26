using System.Collections.Generic;
using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    public List<Transform> PlayersOnElevator = new();

    private void OnTriggerEnter(Collider other)
    {
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

        Debug.Log("Players on elevator: " + PlayersOnElevator.Count);
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterController controller =
            other.GetComponentInParent<CharacterController>();

        if (controller == null)
            return;

        Transform player = controller.transform;

        if (PlayersOnElevator.Contains(player))
        {
            PlayersOnElevator.Remove(player);
            Debug.Log("Player left elevator: " + player.name);
        }

        Debug.Log("Players on elevator: " + PlayersOnElevator.Count);
    }
}