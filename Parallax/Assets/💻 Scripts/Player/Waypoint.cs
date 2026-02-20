using UnityEngine; 

public class Waypoint : MonoBehaviour { 
    [SerializeField] private string waypointTag = "Waypoint"; 
    [SerializeField] private FollowCam followCam; // drag your camera here 
    [SerializeField] private Transform secondaryTarget; // drag your cat root here 
    private bool completed = false; 
    private GameObject lastBox; 
    private void OnTriggerEnter(Collider other) { 
        if (completed) return; 
        var root = other.transform.root; 
        if (!root.CompareTag(waypointTag)) return; 
        completed = true; 
        lastBox = root.gameObject; 
        Debug.Log("Waypoint reached!"); 
        Invoke(nameof(LockLastBox), 0.8f); 
    } 
    private void LockLastBox() { 
        if (lastBox) OnWaypointCompleted(lastBox); 
    } 
    private void OnWaypointCompleted(GameObject box) { 
        var rb = box.GetComponent<Rigidbody>(); 
        if (rb != null) { 
            rb.linearVelocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero; 
            rb.isKinematic = true; 
        } 
        // ✅ Switch follow target to secondary target 
        if (followCam && secondaryTarget) followCam.SetTarget(secondaryTarget, snapToDefaultAngles: false); 
        else Debug.LogWarning("Missing followCam or secondaryTarget reference on Waypoint."); 
    }
}