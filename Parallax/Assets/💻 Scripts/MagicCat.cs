using UnityEngine;

//TODO:FIXME DONT USE THIS, this is for at test ONLY 

public class MagicCat : MonoBehaviour
{
    private void Start()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Player"))
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                rb.isKinematic = false;
            }
        }
    }
}