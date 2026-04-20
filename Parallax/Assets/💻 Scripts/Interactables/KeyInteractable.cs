using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class KeyInteractable : NetworkBehaviour, IInteractable
{
    [SerializeField] private string keyId = "ButtonKey";
    [SerializeField] private AudioClip keySound;

    private AudioSource audioSource;
    private Rigidbody rb;

    private Transform holder;
    private Transform holdPoint;

    private bool isHeld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact(GameObject interactor)
    {
        if (!IsSpawned) return;

        if (isHeld)
        {
            Drop();
        }
        else
        {
            Pickup(interactor);
        }
    }

    private void Pickup(GameObject player)
    {
        MouthCarryPoint carry = player.GetComponent<MouthCarryPoint>();
        if (carry == null) return;

        holder = player.transform;
        holdPoint = carry.MouthPoint;

        isHeld = true;

        rb.useGravity = false;
        rb.isKinematic = true;

        if (keySound && audioSource)
            audioSource.PlayOneShot(keySound);
    }

    private void Drop()
    {
        isHeld = false;

        holder = null;
        holdPoint = null;

        rb.useGravity = true;
        rb.isKinematic = false;
    }

    private void LateUpdate()
    {
        if (!isHeld || holdPoint == null) return;

        transform.position = holdPoint.position;
        transform.rotation = holdPoint.rotation;
    }

    public string GetInteractText()
    {
        return isHeld ? "Drop key" : "Pick up key";
    }
}