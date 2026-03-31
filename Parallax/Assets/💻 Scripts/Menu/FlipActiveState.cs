using System.Collections.Generic;
using UnityEngine;

public class FlipActiveState : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToFlip;

    public void Flip()
    {
        foreach (GameObject obj in objectsToFlip)
        {
            obj.SetActive(!obj.activeSelf); 
        }
    }
}
