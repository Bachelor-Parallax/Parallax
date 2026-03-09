using UnityEngine;

public class BoxInteraction : MonoBehaviour, IInteract
{
    [Header("Detection")]
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask boxLayer;

    [Header("Carry")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float moveToHoldSpeed = 12f;

    [Header("Pull/Push")]
    [SerializeField] private float attachDistance = 1.2f;

    private BoxInteractable nearbyBox;
    private BoxInteractable heldBox;
    private BoxInteractable attachedBox;

    private void Update()
    {
        FindNearbyBox();
    }

    public void Interact()
    {
        Debug.Log("Interact called");

        if (heldBox != null)
        {
            DropHeldBox();
            return;
        }

        if (attachedBox != null)
        {
            DetachBox();
            return;
        }

        if (nearbyBox != null)
        {
            Debug.Log($"Trying to interact with {nearbyBox.name}");

            if (nearbyBox.CanLift)
            {
                LiftBox(nearbyBox);
            }
            else if (nearbyBox.CanPushOrPull)
            {
                AttachBox(nearbyBox);
            }
        }
        else
        {
            Debug.Log("No nearby box found");
        }
    }

    private void LateUpdate()
    {
        if (heldBox != null)
        {
            Vector3 targetPos = holdPoint != null
                ? holdPoint.position
                : heldBox.GetCarryPosition(transform);

            heldBox.transform.position = Vector3.Lerp(
                heldBox.transform.position,
                targetPos,
                Time.deltaTime * moveToHoldSpeed
            );

            heldBox.transform.rotation = Quaternion.Lerp(
                heldBox.transform.rotation,
                Quaternion.identity,
                Time.deltaTime * moveToHoldSpeed
            );
        }

        if (attachedBox != null)
        {
            Vector3 targetPos = transform.position + transform.forward * attachDistance;
            targetPos.y = attachedBox.transform.position.y;

            Rigidbody rb = attachedBox.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
            {
                Vector3 dir = targetPos - attachedBox.transform.position;
                rb.linearVelocity = new Vector3(dir.x * 5f, rb.linearVelocity.y, dir.z * 5f);
            }
        }
    }

    private void FindNearbyBox()
    {
        nearbyBox = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, boxLayer);

        float closest = float.MaxValue;

        foreach (var hit in hits)
        {
            BoxInteractable box = hit.GetComponentInParent<BoxInteractable>();
            if (box == null) continue;

            float dist = Vector3.Distance(transform.position, box.transform.position);
            if (dist < closest)
            {
                closest = dist;
                nearbyBox = box;
            }
        }
    }

    private void LiftBox(BoxInteractable box)
    {
        heldBox = box;
        attachedBox = null;
        box.SetHeld(true);
    }

    private void DropHeldBox()
    {
        heldBox.SetHeld(false);
        heldBox = null;
    }

    private void AttachBox(BoxInteractable box)
    {
        attachedBox = box;
        heldBox = null;
    }

    private void DetachBox()
    {
        attachedBox = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}