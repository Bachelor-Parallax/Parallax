using UnityEngine;

public class AsymmetricalMeshRenderer : BaseAsymmetricalComponent<MeshRenderer>
{
    public override void ApplyPerspectiveProfile(PerspectiveProfile profile)
    {
        if (AsymComponent == null) return;
        AsymComponent.enabled = Profile == profile;
    }
}