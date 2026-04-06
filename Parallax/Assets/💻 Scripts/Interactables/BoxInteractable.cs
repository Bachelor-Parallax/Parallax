using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoxInteractable : MonoBehaviour
{
    [Header("Box Type")]
    [SerializeField] private BoxSize boxSize = BoxSize.Small;

    [Header("Carry Settings")]
    [SerializeField] private Transform carryPointOverride;
    [SerializeField] private Vector3 holdOffset = new Vector3(0f, 1.2f, 0.8f);

    private Rigidbody rb;

    public BoxSize BoxSize => boxSize;
    public bool CanLift => boxSize == BoxSize.Small;
    public bool CanPushOrPull => boxSize == BoxSize.Large || boxSize == BoxSize.Small;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // For large boxes, we want to prevent them from tipping over when pushed or pulled
        if (boxSize == BoxSize.Large)
        {
            rb.constraints |= RigidbodyConstraints.FreezeRotationX;
            rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
            rb.linearDamping = 4f;
        }
    }

    public void SetHeld(bool held)
    {
        if (rb == null) return;

        rb.useGravity = !held;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.freezeRotation = held;

        // mens den bæres, vil man ofte gerne undgå at fysikken slår den væk
        rb.isKinematic = held;
    }

    public Vector3 GetCarryPosition(Transform player)
    {
        if (carryPointOverride != null)
            return carryPointOverride.position;

        return player.position
             + player.forward * holdOffset.z
             + Vector3.up * holdOffset.y
             + player.right * holdOffset.x;
    }
}