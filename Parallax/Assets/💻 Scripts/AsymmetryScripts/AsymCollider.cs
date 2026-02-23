using UnityEngine;

public class AsymCollider : BaseAsymProperty<Collider>
{
    [SerializeField] private PerspectiveProfile _profile;

    public override void ApplyPerspectiveProfile(PerspectiveProfile profile)
    {
        string targetTag = _profile == PerspectiveProfile.A ? "B" : "A";
        GameObject[] objs = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (GameObject obj in objs)
        {
            Collider c = obj.GetComponent<Collider>();
            if (c == null) continue;
            Physics.IgnoreCollision(c, AsymComponent);
        }
    }
}