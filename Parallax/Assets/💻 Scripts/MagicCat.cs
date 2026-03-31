using UnityEngine;

//TODO:FIXME DONT USE THIS, this is for at test ONLY 

public class MagicCat : MonoBehaviour
{
    private void LateUpdate()
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