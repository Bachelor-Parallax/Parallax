using System.Collections;
using UnityEngine;

public class TeleCat : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(TeleportAfterDelay());
    }

    IEnumerator TeleportAfterDelay()
    {
        // Wait for scene to fully initialize
        yield return null;

        // Extra delay (seconds)
        yield return new WaitForSeconds(2f);

        TeleportAllPlayers();
    }

    void TeleportAllPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            player.transform.position = new Vector3(0f, 3f, 0f);
        }
    }
}