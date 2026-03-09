using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PushRigidbodies : MonoBehaviour
{
    [SerializeField] private float pushPower = 2f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var rb = hit.collider.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;

        
        if (hit.moveDirection.y < -0.3f) return;

      
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        rb.AddForce(pushDir * pushPower, ForceMode.Impulse);
    }
}