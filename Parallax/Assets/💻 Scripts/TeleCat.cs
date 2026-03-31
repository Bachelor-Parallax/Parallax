using System;
using UnityEngine;

public class TeleCat : MonoBehaviour
{
    private void Awake()
    {
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
