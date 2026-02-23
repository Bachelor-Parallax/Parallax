using UnityEngine;

public class AsymRenderer : BaseAsymProperty<Renderer>
{
    public override void ApplyPerspectiveProfile(PerspectiveProfile profile)
    {
        AsymComponent.enabled = Profile == profile;
    }
}