using UnityEngine;

public class KeyInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string keyId = "gold_key";

    public void Interact(GameObject interactor)
    {
        Debug.Log($"Picked up key: {keyId}");
        Destroy(gameObject);
    }

    public string GetInteractText()
    {
        return "Pick up key";
    }
}