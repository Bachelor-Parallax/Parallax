using UnityEngine;

public class AsymmetricalLight : BaseAsymmetricalComponent<Light>
{
    public override void ApplyPerspectiveProfile(PerspectiveProfile profile)
    {
        if (AsymComponent == null) return;
        AsymComponent.enabled = Profile == profile;
    }
}