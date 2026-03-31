using System.Collections;
using UnityEngine;

public class MagicCat : MonoBehaviour
{
    private void Start()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Player"))
            {
                obj.SetActive(true);
            }
        }
    }
}