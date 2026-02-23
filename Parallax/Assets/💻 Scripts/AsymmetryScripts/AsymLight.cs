using UnityEngine;

public class AsymLight : BaseAsymProperty<Light>
{
    [SerializeField] private PerspectiveProfile _profile;

    public override void ApplyPerspectiveProfile(PerspectiveProfile profile)
    {
        AsymComponent.enabled = _profile == profile;
    }
}