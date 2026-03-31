using UnityEngine;

public class AsymVisibility : BaseAsymProperty<Renderer>
{
    [SerializeField] private PerspectiveProfile _profile;

    public override void ApplyPerspectiveProfile(PerspectiveProfile profile)
    {
        AsymComponent.enabled = _profile == profile;
    }
}