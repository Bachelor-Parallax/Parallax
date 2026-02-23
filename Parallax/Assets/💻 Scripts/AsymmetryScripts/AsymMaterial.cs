using UnityEngine;

public class AsymMaterial : BaseAsymProperty<Renderer>
{
    [SerializeField] private Material _materialA;
    [SerializeField] private Material _materialB;

    public override void ApplyPerspectiveProfile(PerspectiveProfile profile)
    {
        AsymComponent.material = profile == PerspectiveProfile.A ? _materialA : _materialB;
    }
}