using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class SmallBoxCarryable : MonoBehaviour, IInteractable
{
    [Header("Carry")]
    [SerializeField] private Vector3 holdOffset = new Vector3(0f, 0f, 0f);
    [SerializeField] private float carryMoveSpeed = 12f;

    private Rigidbody rb;
    private Collider boxCollider;

    private Transform holder;
    private Transform holdPoint;
    private Collider[] ignoredColliders;

    private bool isHeld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<Collider>();
    }
    
    private void LateUpdate()
    {
        if (!isHeld || holdPoint == null)
            return;

        Vector3 target = holdPoint.position + holdOffset;
        
        transform.position = target;
        transform.rotation = holdPoint.rotation;
    }

    public bool CanInteract(GameObject interactor)
    {
        RoleController role = interactor.GetComponent<RoleController>();

        if (role == null || !role.IsHuman)
            return false;

        return !isHeld || interactor.transform == holder;
    }

    public void Interact(GameObject interactor)
    {
        if (isHeld)
        {
            if (interactor.transform == holder)
                Drop(interactor);
        }
        else
        {
            Pickup(interactor);
        }
    }

    private void Pickup(GameObject interactor)
    {
        holder = interactor.transform;

        MouthCarryPoint carryPoint = interactor.GetComponentInChildren<MouthCarryPoint>();
        if (carryPoint == null) return;

        holdPoint = carryPoint.transform;
        isHeld = true;

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        ignoredColliders = interactor.GetComponentsInChildren<Collider>();

        foreach (Collider col in ignoredColliders)
            Physics.IgnoreCollision(boxCollider, col, true);

        Interactor playerInteractor = interactor.GetComponent<Interactor>();
        playerInteractor?.SetActiveInteractable(this);
    }

    private void Drop(GameObject interactor)
    {
        isHeld = false;

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;

        if (ignoredColliders != null)
        {
            foreach (Collider col in ignoredColliders)
                Physics.IgnoreCollision(boxCollider, col, false);
        }

        holder = null;
        holdPoint = null;

        Interactor playerInteractor = interactor.GetComponent<Interactor>();
        playerInteractor?.ClearActiveInteractable(this);
    }

    public string GetInteractText()
    {
        return isHeld ? "Drop box" : "Pick up box";
    }
}