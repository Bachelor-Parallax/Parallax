using UnityEngine;

public class AsymLight : BaseAsymProperty<Light>
{
    public override void ApplyPerspectiveProfile(PerspectiveProfile profile)
    {
        AsymComponent.enabled = Profile == profile;
    }
}