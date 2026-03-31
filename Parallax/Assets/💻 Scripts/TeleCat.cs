using System.Collections;
using UnityEngine;

public class TeleCat : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(TeleportAfterDelay());
    }

    private IEnumerator TeleportAfterDelay()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        while (players.Length < 2)
        {
            yield return null;
            players = GameObject.FindGameObjectsWithTag("Player");
        }

        yield return new WaitForSeconds(2f);

        foreach (GameObject player in players)
        {
            player.transform.position = new Vector3(0f, 3f, 0f);
        }
    }
}